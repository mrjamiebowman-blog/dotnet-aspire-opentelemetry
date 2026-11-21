using Microsoft.AspNetCore.Mvc;
using MrJB.OTel.Api.BFF.Clients;
using MrJB.OTel.Api.BFF.Models;
using System.Diagnostics;

namespace MrJB.OTel.Api.BFF.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController(
    CustomersClient customersClient,
    ProductsClient productsClient,
    ILogger<CustomersController> logger) : ControllerBase
{
    /// <summary>
    /// Aggregates a customer (Customers API) with featured products (Products API)
    /// into a single payload used to build a customer email.
    /// </summary>
    [HttpGet("email", Name = nameof(GetCustomerForEmail))]
    [ProducesResponseType<CustomerEmail>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<CustomerEmail>> GetCustomerForEmail(CancellationToken cancellationToken)
    {
        using var activity = OTel.ActivitySource.StartActivity($"{nameof(CustomersController)}.{nameof(GetCustomerForEmail)}");

        // fan out to both downstream apis in parallel
        var customerTask = customersClient.GetCustomerAsync(cancellationToken);
        var productsTask = productsClient.GetFeaturedProductsAsync(cancellationToken);

        try
        {
            await Task.WhenAll(customerTask, productsTask);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddException(ex);
            logger.LogError(ex, "Failed to aggregate customer email data");
            return Problem("Unable to load customer email data from downstream services.", statusCode: StatusCodes.Status502BadGateway);
        }

        var customer = customerTask.Result;
        if (customer is null)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Customers API returned no customer.");
            return Problem("Customers API returned no customer.", statusCode: StatusCodes.Status502BadGateway);
        }

        var result = new CustomerEmail
        {
            Customer = customer,
            FeaturedProducts = productsTask.Result,
        };

        activity?.SetTag("customer.id", customer.CustomerId);
        activity?.SetTag("customer.email", customer.Email);
        activity?.SetTag("products.featured.count", result.FeaturedProducts.Count);
        activity?.SetStatus(ActivityStatusCode.Ok);

        OTel.Meters.AddGetCustomerEmail();
        logger.LogInformation("Built customer email for {CustomerId} with {ProductCount} featured products",
            customer.CustomerId, result.FeaturedProducts.Count);

        return Ok(result);
    }
}
