namespace MrJB.OTel.Domain.Models;

public class Product
{
    public int ProductId { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public bool IsFeatured { get; set; }
}
