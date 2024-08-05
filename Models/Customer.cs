
namespace Models;
public class Customer
{
    public int CustomerId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required Address[] Address { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}
