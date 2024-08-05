using FluentAssertions;
using Models;
using Models.Enums;
using Models.IRepositories;
using Moq;
using Services;

namespace ServicesUnitTests;
public class AccountServiceTests
{
    private Mock<Account> _mockAccount = new();
    private Mock<IAccountRepository> _mockAccountRepository = new();
    private AccountService _accountService;

    private readonly int _badCustomerId = 0;
    private readonly int _customerId = 1;
    private readonly int _accountId = 1;
    private readonly decimal _baseAccountBalance = 130;
    private readonly decimal _depositAmount = 40;
    private readonly decimal _withdrawalAmount = 50;

    public AccountServiceTests()
    {
        _accountService = new(_mockAccountRepository.Object,
            new AccountCreateValidator(),
            new AccountUpdateValidator(),
            new AccountBalanceUpdateValidator());
    }

    /*
        I wrote a set of unit tests just for the create endpoint to show how I would
          approach unit testing without spending too much on creating them
          for all endpoints.
    */

    #region CreateAccountAsync
    [Fact]
    public async Task CreateAccountAsync_ShouldFail_RequestIsNull()
    {
        var result = await _accountService.CreateAccountAsync(null);

        result.Error.Should().Be(AccountServiceError.BadRequest);
    }

    [Fact]
    public async Task CreateAccountAsync_ShouldFail_RequestInvalid()
    {
        var result = await _accountService.CreateAccountAsync(_mockAccount.Object);

        result.Error.Should().Be(AccountServiceError.RequestNotValid);
    }

    [Fact]
    public async Task CreateAccountAsync_ShouldFail_NoAccountsAndTryingToCreateChecking()
    {
        _mockAccount.Setup(x => x.CustomerId).Returns(_customerId);
        _mockAccount.Setup(x => x.AccountTypeId).Returns((int)AccountType.Checking);
        _mockAccount.Setup(x => x.AccountBalance).Returns(_baseAccountBalance);

        _mockAccountRepository.Setup(x => x.GetAllByCustomerIdAsync(_customerId))
            .Returns(Task.FromResult(ArraySegment<Account>.Empty as IReadOnlyList<Account>));

        var result = await _accountService.CreateAccountAsync(_mockAccount.Object);

        result.Error.Should().Be(AccountServiceError.MissingSavingsAccount);
    }

    [Fact]
    public async Task CreateAccountAsync_ShouldFail_HasAccountsButMissingOneSavings()
    {
        _mockAccount.Setup(x => x.CustomerId).Returns(_customerId);
        _mockAccount.Setup(x => x.AccountTypeId).Returns((int)AccountType.Checking);
        _mockAccount.Setup(x => x.AccountBalance).Returns(_baseAccountBalance);

        Account[] accounts = [_mockAccount.Object, _mockAccount.Object];

        _mockAccountRepository.Setup(x => x.GetAllByCustomerIdAsync(_customerId))
            .Returns(Task.FromResult(accounts as IReadOnlyList<Account>));

        var result = await _accountService.CreateAccountAsync(_mockAccount.Object);

        result.Error.Should().Be(AccountServiceError.MissingSavingsAccount);
    }

    [Fact]
    public async Task CreateAccountAsync_ShouldFail_FailedToCreateAccount()
    {
        _mockAccount.Setup(x => x.CustomerId).Returns(_customerId);
        _mockAccount.Setup(x => x.AccountTypeId).Returns((int)AccountType.Checking);
        _mockAccount.Setup(x => x.AccountBalance).Returns(_baseAccountBalance);
        Account savings = new() 
        {
            AccountTypeId = (int)AccountType.Savings
        };

        Account[] accounts = [_mockAccount.Object, savings];

        _mockAccountRepository.Setup(x => x.GetAllByCustomerIdAsync(_customerId))
            .Returns(Task.FromResult(accounts as IReadOnlyList<Account>));
        _mockAccountRepository.Setup(x => x.CreateAccountAsync(_mockAccount.Object))
            .Returns(Task.FromResult(0));

        var result = await _accountService.CreateAccountAsync(_mockAccount.Object);

        result.Error.Should().Be(AccountServiceError.FailedToCreateAccount);
    }

    [Theory]
    [InlineData([AccountType.Savings, true])]
    [InlineData([AccountType.Checking, false])]
    [InlineData([AccountType.Savings, false])]
    public async Task CreateAccountAsync_ShouldPass(AccountType accountType, bool firstAccount)
    {
        _mockAccount.Setup(x => x.CustomerId).Returns(_customerId);
        _mockAccount.Setup(x => x.AccountTypeId).Returns((int)accountType);
        _mockAccount.Setup(x => x.AccountBalance).Returns(_baseAccountBalance);

        if (!firstAccount)
        {
            Account savings = new() { AccountTypeId = (int)AccountType.Savings };
            Account[] accounts = [savings];

            _mockAccountRepository.Setup(x => x.GetAllByCustomerIdAsync(_customerId))
                .Returns(Task.FromResult(accounts as IReadOnlyList<Account>));
        }
        else
        {
            _mockAccountRepository.Setup(x => x.GetAllByCustomerIdAsync(_customerId))
                            .Returns(Task.FromResult(new List<Account>() as IReadOnlyList<Account>));
        }


        _mockAccountRepository.Setup(x => x.CreateAccountAsync(_mockAccount.Object))
            .Returns(Task.FromResult(_accountId));

        var result = await _accountService.CreateAccountAsync(_mockAccount.Object);

        result.Data.Should().BeEquivalentTo(_mockAccount.Object);
    }
    #endregion
}
