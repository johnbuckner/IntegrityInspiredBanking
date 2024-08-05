namespace Api.Models;

public class AccountResponse
{
    public int? CustomerId { get; set; }
    public int? AccountId { get; set; }
    public int? AccountTypeId { get; set; }
    public decimal? Balance { get; set; }
    public bool? Succeeded { get; set; }
}
