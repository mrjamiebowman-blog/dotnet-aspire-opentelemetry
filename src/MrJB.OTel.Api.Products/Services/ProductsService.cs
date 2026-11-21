using MrJB.OTel.Domain.Models;

namespace MrJB.OTel.Api.Products.Services;

public class ProductsService(ILogger<ProductsService> logger) : IProductsService
{
    private static readonly Product[] Catalog =
    [
        new() { ProductId = 1001, Name = "Trace Hoodie", Description = "Warm hoodie for long debugging sessions.", Price = 49.99m, IsFeatured = true },
        new() { ProductId = 1002, Name = "Span Mug", Description = "Holds exactly one cup of coffee per span.", Price = 14.99m, IsFeatured = true },
        new() { ProductId = 1003, Name = "Metrics Notebook", Description = "Dot-grid notebook for counters and histograms.", Price = 9.99m, IsFeatured = false },
        new() { ProductId = 1004, Name = "Collector Sticker Pack", Description = "Ten stickers for your laptop lid.", Price = 4.99m, IsFeatured = true },
        new() { ProductId = 1005, Name = "Baggage Tote", Description = "Carries context across every hop.", Price = 19.99m, IsFeatured = true },
        new() { ProductId = 1006, Name = "Exporter Cap", Description = "Ships your style over OTLP.", Price = 24.99m, IsFeatured = false },
    ];

    public async Task<IReadOnlyList<Product>> GetFeaturedProductsAsync(CancellationToken cancellationToken = default)
    {
        using var activity = OTel.ActivitySource.StartActivity($"{nameof(ProductsService)}.{nameof(GetFeaturedProductsAsync)}");

        // simulate a catalog lookup so the span has some duration
        await Task.Delay(Random.Shared.Next(50, 300), cancellationToken);

        var featured = Catalog.Where(p => p.IsFeatured).ToList();

        activity?.SetTag("products.featured.count", featured.Count);
        logger.LogInformation("Retrieved {Count} featured products", featured.Count);

        return featured;
    }
}
