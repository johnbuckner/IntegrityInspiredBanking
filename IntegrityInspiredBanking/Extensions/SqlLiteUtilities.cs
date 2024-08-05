using Dapper;
using Microsoft.Data.Sqlite;

namespace Api.Extensions;

public static class SqlLiteUtilities
{
    public static async Task<bool> InitializeDBAsync(this WebApplication app)
    {
        string connString = app.Configuration.GetConnectionString("SqlLite")!;

        var createSql = @"
            CREATE TABLE [Accounts] (
                [AccountId] INTEGER NOT NULL
                ,[CustomerId] bigint NOT NULL
                ,[AccountTypeId] bigint NOT NULL
                ,[AccountBalance] numeric(53,0) DEFAULT (0.00) NOT NULL
                ,[IsActive] bigint DEFAULT (1) NOT NULL
                ,[OpenedOn] text NOT NULL
                ,[ClosedOn] text NULL
                ,CONSTRAINT [sqlite_autoindex_Accounts_1] PRIMARY KEY ([AccountId] AUTOINCREMENT)
            );";

        using var conn = new SqliteConnection(connString);
        conn.Open();
        using var transaction = conn.BeginTransaction();

        try
        {
            await conn.ExecuteAsync(createSql, transaction);

            var tableExists = await conn.QueryFirstOrDefaultAsync<int>(
                "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='Accounts';", transaction);

            if (tableExists < 0)
            {
                return false;
            }

            transaction.Commit();
            conn.Close();
            return true;
        }
        catch (Exception)
        {
            transaction.Rollback();
            conn.Close();
            return false;
        }
    }
}
