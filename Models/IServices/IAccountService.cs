using Models.Enums;

namespace Models.IServices;
public interface IAccountService
{
    Task<ServiceResult<IReadOnlyList<Account>, AccountServiceError>> GetAccountsByCustomerIdAsync(int customerId);
    Task<ServiceResult<Account, AccountServiceError>> CreateAccountAsync(Account request);
    Task<ServiceResult<Account, AccountServiceError>> SoftDeleteAccountAsync(Account request);
    Task<ServiceResult<Account, AccountServiceError>> DepositMoneyIntoAccountAsync(AccountBalanceUpdate request);
    Task<ServiceResult<Account, AccountServiceError>> WithdrawMoneyFromAccountAsync(AccountBalanceUpdate request);
}
