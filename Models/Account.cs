using FluentValidation;
using Models.IValidators;

namespace Models;
public class Account
{
    public virtual int AccountId { get; set; }
    public virtual int CustomerId { get; init; }
    public virtual int AccountTypeId { get; init; }
    public virtual decimal AccountBalance { get; set; }
    public virtual decimal MinimumBalance { get; init; } = 100;
    public virtual bool IsActive { get; set; } = true;
    public virtual DateTime OpenedOn { get; init; } = DateTime.UtcNow;
    public virtual DateTime? ClosedOn { get; set; }
}

public class AccountCreateValidator : AbstractValidator<Account>, ICreateValidator<Account>
{
    public AccountCreateValidator()
    {
        RuleFor(x => x.CustomerId).NotNull().GreaterThan(0);
        RuleFor(x => x.AccountTypeId).NotNull().InclusiveBetween(1, 2).WithMessage("Account type requested not in system.");
        RuleFor(x => x.AccountBalance).NotNull().GreaterThanOrEqualTo(100).WithMessage("Account balance must at least by $100 to create.");
    }
}

public class AccountUpdateValidator : AbstractValidator<Account>, IUpdateValidator<Account>
{
    public AccountUpdateValidator()
    {
        RuleFor(x => x.CustomerId).NotNull().GreaterThan(0);
        RuleFor(x => x.AccountId).NotNull().GreaterThan(0);
        RuleFor(x => x.AccountTypeId).NotNull().InclusiveBetween(1, 2).WithMessage("Account type requested not in system.");
    }
}
