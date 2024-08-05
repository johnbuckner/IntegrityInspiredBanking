using Api.Models;
using Models;

namespace Api.ResponseMethods;

public static class AccountResponses
{
    public static AccountResponse ToCreateResponse(this Account data)
    {
        AccountResponse response = new()
        {
            CustomerId = data.CustomerId,
            AccountId = data.AccountId,
            AccountTypeId = data.AccountTypeId,
            Balance = data.AccountBalance,
            Succeeded = true
        };

        return response;
    }

    public static AccountResponse ToUpdateResponse(this Account data)
    {
        AccountResponse response = new()
        {
            CustomerId = data.CustomerId,
            AccountId = data.AccountId,
            Balance = data.AccountBalance,
            Succeeded = true
        };

        return response;
    }

    public static AccountResponse ToDeleteResponse(this Account data)
    {
        AccountResponse response = new()
        {
            CustomerId = data.CustomerId,
            AccountId = data.AccountId,
            Succeeded = true
        };

        return response;
    }
}
