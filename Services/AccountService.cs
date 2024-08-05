using FluentValidation;
using Models;
using Models.Enums;
using Models.IRepositories;
using Models.IServices;
using Models.IValidators;

namespace Services;
public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepo;
    private readonly ICreateValidator<Account> _createValidator;
    private readonly IUpdateValidator<Account> _updateValidator;
    private readonly IValidator<AccountBalanceUpdate> _balanceUpdateValidator;

    public AccountService(
        IAccountRepository accountRepo,
        ICreateValidator<Account> createValidator,
        IUpdateValidator<Account> updateValidator,
        IValidator<AccountBalanceUpdate> balanceUpdateValidator)
    {
        _accountRepo = accountRepo;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _balanceUpdateValidator = balanceUpdateValidator;
    }

    public async Task<ServiceResult<IReadOnlyList<Account>, AccountServiceError>> GetAccountsByCustomerIdAsync(int customerId)
    {
        // check for customer's existence

        if (customerId < 1) return new(AccountServiceError.BadRequest);

        IReadOnlyList<Account> accounts = await _accountRepo.GetAllByCustomerIdAsync(customerId);

        return new(accounts);
    }

    public async Task<ServiceResult<Account, AccountServiceError>> CreateAccountAsync(Account request)
    {
        if (request is null) return new(AccountServiceError.BadRequest);

        var validation = await _createValidator.ValidateAsync(instance: request);
        if (!validation.IsValid) return new(AccountServiceError.RequestNotValid);

        var customerAccounts = await _accountRepo.GetAllByCustomerIdAsync(request.CustomerId);

        if (customerAccounts.Count == 0 && request.AccountTypeId == (int)AccountType.Checking 
            || customerAccounts.Count > 0 && !customerAccounts.Any(x => x.AccountTypeId == (int)AccountType.Savings))
        {
            return new(AccountServiceError.MissingSavingsAccount);
        }

        int newAccountId = await _accountRepo.CreateAccountAsync(request);
        if (newAccountId == 0) return new(AccountServiceError.FailedToCreateAccount);

        request.AccountId = newAccountId;

        return new(request);
    }

    public async Task<ServiceResult<Account, AccountServiceError>> SoftDeleteAccountAsync(Account request)
    {
        if (request.CustomerId < 1 || request.AccountId < 1) return new(AccountServiceError.BadRequest);

        Account? account = await _accountRepo.GetByCustomerIdAndAccountIdAsync(request.CustomerId, request.AccountId);

        if (account is null) return new(AccountServiceError.AccountNotFound);
        if (!account.IsActive) return new(AccountServiceError.AccountDeleted);
        if (account.AccountBalance != 0) return new(AccountServiceError.FundsStillAvailable);

        int recordsUpdated = await _accountRepo.SoftDeleteAccountByIdAsync(account.AccountId);
        if (recordsUpdated == 0) return new(AccountServiceError.FailedToCloseAccount);

        return new(account);
    }

    public async Task<ServiceResult<Account, AccountServiceError>> DepositMoneyIntoAccountAsync(AccountBalanceUpdate request)
    {
        if (request is null) return new(AccountServiceError.BadRequest);

        var validation = await _balanceUpdateValidator.ValidateAsync(request);
        if (!validation.IsValid) return new(AccountServiceError.RequestNotValid);

        Account? account = await _accountRepo.GetByCustomerIdAndAccountIdAsync(request.CustomerId, request.AccountId);
        if (account is null) return new(AccountServiceError.AccountNotFound);
        if (!account.IsActive) return new(AccountServiceError.AccountDeleted);

        account.AccountBalance += request.Amount;

        int recordsUpdated = await _accountRepo.UpdateAccountBalanceByIdAsync(account.AccountId, account.AccountBalance);
        if (recordsUpdated == 0) return new(AccountServiceError.FailedToDepositToAccount);

        return new(account);
    }

    public async Task<ServiceResult<Account, AccountServiceError>> WithdrawMoneyFromAccountAsync(AccountBalanceUpdate request)
    {
        if (request is null) return new(AccountServiceError.BadRequest);

        var validation = await _balanceUpdateValidator.ValidateAsync(request);
        if (!validation.IsValid) return new(AccountServiceError.RequestNotValid);

        Account? account = await _accountRepo.GetByCustomerIdAndAccountIdAsync(request.CustomerId, request.AccountId);
        if (account is null) return new(AccountServiceError.AccountNotFound);
        if (!account.IsActive) return new(AccountServiceError.AccountDeleted);

        account.AccountBalance -= request.Amount;
        if (account.AccountBalance < 0) return new(AccountServiceError.WithdrawalWillOverdraftAccount);
       
        int recordsUpdated = await _accountRepo.UpdateAccountBalanceByIdAsync(account.AccountId, account.AccountBalance);
        if (recordsUpdated == 0) return new(AccountServiceError.FailedToWithdrawFromAccount);

        return new(account);
    }
}
