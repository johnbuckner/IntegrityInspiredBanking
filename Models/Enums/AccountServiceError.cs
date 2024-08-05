namespace Models.Enums;
public enum AccountServiceError
{
    BadRequest = 1,
    RequestNotValid,
    AccountNotFound,
    FailedToCreateAccount,
    FailedToCloseAccount,
    FailedToDepositToAccount,
    FailedToWithdrawFromAccount,
    FundsStillAvailable,
    MissingSavingsAccount,
    WithdrawalWillOverdraftAccount
}
