using MrJB.OTel.Domain.Models;

namespace MrJB.OTel.Api.BFF.Models;

/// <summary>
/// Aggregated view model the BFF returns to build a customer email:
/// the customer (from the Customers API) plus featured products (from the Products API).
/// </summary>
public class CustomerEmail
{
    public required Customer Customer { get; init; }

    public required IReadOnlyList<Product> FeaturedProducts { get; init; }

    public DateTimeOffset GeneratedAt { get; init; } = DateTimeOffset.UtcNow;
}
