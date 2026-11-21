using MrJB.OTel.Domain.Models;

namespace MrJB.OTel.Api.Products.Services;

public interface IProductsService
{
    Task<IReadOnlyList<Product>> GetFeaturedProductsAsync(CancellationToken cancellationToken = default);
}
