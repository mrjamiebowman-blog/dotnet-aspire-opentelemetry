namespace MrJB.OpenTelemetry.Domain.Models;

public class Customer
{
    public int? CustomerId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public List<Order> Orders { get; set; }

}
