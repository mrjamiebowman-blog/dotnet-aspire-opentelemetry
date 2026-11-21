using Microsoft.AspNetCore.Mvc;
using MrJB.OTel.Api.Customers.Services;
using MrJB.OTel.Domain.Models;
using System.Diagnostics;

namespace MrJB.OTel.Api.Customers.Controllers;

[ApiController]
[Route("customers")]
public class CustomersController(ICustomersService customersService) : ControllerBase
{
    [HttpPost(Name = nameof(GetCustomers))]
    public async Task<Customer> GetCustomers(CancellationToken cancellationToken)
    {
        using var activity = OTel.ActivitySource.StartActivity($"{nameof(CustomersController)}.{nameof(GetCustomers)}");

        try
        {
            var customer = await customersService.GetCustomerAsync(cancellationToken);

            activity?.SetTag("customer.id", customer.CustomerId);
            activity?.SetStatus(ActivityStatusCode.Ok);

            return customer;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddException(ex);
            throw new Exception("Unable to load customers.");
        }
    }
}
