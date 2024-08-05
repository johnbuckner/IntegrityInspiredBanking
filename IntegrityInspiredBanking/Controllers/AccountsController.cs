using Api.ResponseMethods;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Enums;
using Models.IServices;

namespace IntegrityInspiredBanking.Controllers;
[Route("[controller]")]
[ApiController]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        // need logger
        _accountService = accountService;
    }

    // Assuming user has logged in successfully
    // get user accounts
    [HttpGet("{customerId:int}")]
    public async Task<IActionResult> GetUserAccountsByCustomerId(int customerId)
    {
        var result = await _accountService.GetAccountsByCustomerIdAsync(customerId);
        
        if (!result.IsSuccess)
        {
            return result.Error switch
            {
                AccountServiceError.BadRequest => BadRequest(),
                AccountServiceError.AccountNotFound => NotFound()
            };
        }
        return Ok(result.Data);
    }

    // create account
    [HttpPost]
    [ActionName(nameof(CreateUserAccountAsync))]
    public async Task<IActionResult> CreateUserAccountAsync(Account request)
    {
        var result = await _accountService.CreateAccountAsync(request);

        if (!result.IsSuccess)
        {
            return result.Error switch
            {
                AccountServiceError.BadRequest => BadRequest(),
                AccountServiceError.RequestNotValid => BadRequest(),
                AccountServiceError.FailedToCreateAccount => Problem("Failed to create account.", statusCode: StatusCodes.Status500InternalServerError),
                AccountServiceError.MissingSavingsAccount => NotFound("Missing at least one savings account."),
            };
        }
        return CreatedAtAction(nameof(CreateUserAccountAsync), result.Data!.ToCreateResponse());
    }

    // close account
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> SoftDeleteByIdAsync(int id, Account request)
    {
        var result = await _accountService.SoftDeleteAccountAsync(request);

        if (!result.IsSuccess)
        {
            return result.Error switch
            {
                AccountServiceError.BadRequest => BadRequest(),
                AccountServiceError.AccountNotFound => NotFound(),
                AccountServiceError.FundsStillAvailable => BadRequest("Funds are still available in account. Please empty account before closing."),
                AccountServiceError.FailedToCloseAccount => Problem("Failed to delete account.", statusCode: StatusCodes.Status500InternalServerError)
            };
        }
        return Ok(result.Data!.ToDeleteResponse());
    }

    // deposit money
    [HttpPut("{id:int}/deposit")]
    public async Task<IActionResult> DepositMoneyIntoAccountByIdAsync(int id, AccountBalanceUpdate request)
    {
        var result = await _accountService.DepositMoneyIntoAccountAsync(request);

        if (!result.IsSuccess)
        {
            return result.Error switch
            {
                AccountServiceError.BadRequest => BadRequest(),
                AccountServiceError.RequestNotValid => BadRequest(),
                AccountServiceError.AccountNotFound => NotFound(),
                AccountServiceError.FailedToDepositToAccount => Problem("Failed to deposit money to account.", statusCode: StatusCodes.Status500InternalServerError)
            };
        }
        return Ok(result.Data!.ToUpdateResponse());
    }

    // withdraw money
    [HttpPut("{id:int}/withdraw")]
    public async Task<IActionResult> WithdrawMoneyFromAccountByIdAsync(int id, AccountBalanceUpdate request)
    {
        var result = await _accountService.WithdrawMoneyFromAccountAsync(request);

        if (!result.IsSuccess)
        {
            return result.Error switch
            {
                AccountServiceError.BadRequest => BadRequest(),
                AccountServiceError.RequestNotValid => BadRequest(),
                AccountServiceError.AccountNotFound => NotFound(),
                AccountServiceError.FailedToWithdrawFromAccount => Problem("Failed to deposit money to account.", statusCode: StatusCodes.Status500InternalServerError),
                AccountServiceError.WithdrawalWillOverdraftAccount => BadRequest("Withdrawal will overdraft account. Please try again with a different amount"),
            };
        }
        return Ok(result.Data!.ToUpdateResponse());
    }
}
