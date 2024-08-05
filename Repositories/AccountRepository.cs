using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Models;
using Models.IRepositories;

namespace Repositories;
public class AccountRepository : IAccountRepository
{
    private readonly string _sqlLiteConnectionString;

    public AccountRepository(IConfiguration config) =>
        _sqlLiteConnectionString = config.GetConnectionString("SqlLite")!;

    public async Task<IReadOnlyList<Account>> GetAllByCustomerIdAsync(int customerId)
    {
        using var conn = new SqliteConnection(_sqlLiteConnectionString);
        conn.Open();
        List<Account>? result = (await conn.QueryAsync<Account>(
            "SELECT * FROM Accounts WHERE CustomerId=@CustomerId",
            new { CustomerId = customerId })).ToList();
        conn.Close();
        return result ?? [];
    }

    public async Task<Account?> GetByIdAsync(int id) 
    {
        using var conn = new SqliteConnection(_sqlLiteConnectionString);
        conn.Open();
        Account? result = await conn.QueryFirstOrDefaultAsync<Account>(
            "SELECT * FROM Accounts WHERE AccountId=@AccountId",
            new { AccountId = id });
        conn.Close();
        return result;
    }

    public async Task<Account?> GetByCustomerIdAndAccountIdAsync(int customerId, int accountId)
    {
        using var conn = new SqliteConnection(_sqlLiteConnectionString);
        conn.Open();
        Account? result = await conn.QueryFirstOrDefaultAsync<Account>(
            "SELECT * FROM Accounts WHERE CustomerId=@CustomerId AND AccountId=@AccountId",
            new { CustomerId = customerId, AccountId = accountId });
        conn.Close();
        return result;
    }

    public async Task<int> CreateAccountAsync(Account request)
    {
        int newAccountId = 0;
        using var conn = new SqliteConnection(_sqlLiteConnectionString);
        conn.Open();
        var transaction = conn.BeginTransaction();
        try
        {
            newAccountId = await conn.ExecuteScalarAsync<int>(@"
                INSERT INTO Accounts (
                    CustomerId,
                    AccountTypeId,
                    AccountBalance,
                    IsActive,
                    OpenedOn
                )
                VALUES (
                    @CustomerId,
                    @AccountTypeId,
                    @AccountBalance,
                    1,
                    DATETIME());
                SELECT LAST_INSERT_ROWID()", request);
            transaction.Commit();
        }
        catch (SqliteException)
        {
            transaction.Rollback();
        }
        conn.Close();
        return newAccountId;
    }

    public async Task<int> SoftDeleteAccountByIdAsync(int id)
    {
        int recordsUpdated = 0;
        using var conn = new SqliteConnection(_sqlLiteConnectionString);
        conn.Open();
        var transaction = conn.BeginTransaction();
        try
        {
            recordsUpdated = await conn.ExecuteAsync(
                "UPDATE Accounts SET IsActive=0 WHERE AccountId=@AccountId",
                new { AccountId = id });
            transaction.Commit();
        }
        catch (SqliteException)
        {
            transaction.Rollback();
        }
        conn.Close();
        return recordsUpdated;
    }

    public async Task<int> UpdateAccountBalanceByIdAsync(int id, decimal newBalance)
    {
        int recordsUpdated = 0;
        using var conn = new SqliteConnection(_sqlLiteConnectionString);
        conn.Open();
        var transaction = conn.BeginTransaction();
        try
        {
            recordsUpdated = await conn.ExecuteAsync(
                "UPDATE Accounts SET AccountBalance=@NewAccountBalance WHERE AccountId=@AccountId",
                new { AccountId = id, NewAccountBalance = newBalance }, transaction);
            transaction.Commit();
        }
        catch (SqliteException)
        {
            transaction.Rollback();
        }
        conn.Close();
        return recordsUpdated;
    }
}
