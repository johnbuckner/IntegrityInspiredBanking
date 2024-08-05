
using FluentValidation;
using Models.IValidators;

namespace Models;
public class AccountBalanceUpdate
{
    public int CustomerId { get; set; }
    public int AccountId { get; set; }
    public decimal Amount { get; set; }
}

public class AccountBalanceUpdateValidator : AbstractValidator<AccountBalanceUpdate>, IUpdateValidator<AccountBalanceUpdate>
{
    public AccountBalanceUpdateValidator()
    {
        RuleFor(x => x.CustomerId).NotNull().GreaterThan(0);
        RuleFor(x => x.AccountId).NotNull().GreaterThan(0);
        RuleFor(x => x.Amount).NotNull().GreaterThan(0);
    }
}
