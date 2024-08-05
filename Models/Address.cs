
namespace Models;
public class Address
{
    // Sticking with U.S. addresses because I don't know anything about foreign ones
    public required string StreetAddress { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string PostalCode { get; set; }
}
