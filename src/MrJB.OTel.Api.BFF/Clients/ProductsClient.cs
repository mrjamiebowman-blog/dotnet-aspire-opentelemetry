using MrJB.OTel.Domain.Models;

namespace MrJB.OTel.Api.BFF.Clients;

/// <summary>
/// Typed HttpClient for MrJB.OTel.Api.Products. The base address is resolved through
/// service discovery ("api-products").
/// </summary>
public class ProductsClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<Product>> GetFeaturedProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await httpClient.GetFromJsonAsync<List<Product>>("products/featured", cancellationToken);
        return products ?? [];
    }
}
