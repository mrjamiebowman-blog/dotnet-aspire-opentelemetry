using Microsoft.AspNetCore.Mvc;
using MrJB.OTel.Api.Products.Services;
using MrJB.OTel.Domain.Models;
using System.Diagnostics;

namespace MrJB.OTel.Api.Products.Controllers;

[ApiController]
[Route("products")]
public class ProductsController(IProductsService productsService) : ControllerBase
{
    [HttpGet("featured", Name = nameof(GetFeaturedProducts))]
    [ProducesResponseType<IReadOnlyList<Product>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetFeaturedProducts(CancellationToken cancellationToken)
    {
        using var activity = OTel.ActivitySource.StartActivity($"{nameof(ProductsController)}.{nameof(GetFeaturedProducts)}");

        var featured = await productsService.GetFeaturedProductsAsync(cancellationToken);

        OTel.Meters.AddGetFeaturedProducts();
        activity?.SetTag("products.featured.count", featured.Count);
        activity?.SetStatus(ActivityStatusCode.Ok);

        return Ok(featured);
    }
}
