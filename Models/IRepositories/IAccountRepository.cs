
namespace Models.IRepositories;
public interface IAccountRepository
{
    Task<IReadOnlyList<Account>> GetAllByCustomerIdAsync(int customerId);
    Task<Account?> GetByIdAsync(int id);
    Task<Account?> GetByCustomerIdAndAccountIdAsync(int customerId, int accountId);
    Task<int> CreateAccountAsync(Account request);
    Task<int> SoftDeleteAccountByIdAsync(int id);
    Task<int> UpdateAccountBalanceByIdAsync(int id, decimal newBalance);
}
