using MrJB.OTel.Domain.Models;
using RestSharp;
using System.Diagnostics;

namespace MrJB.OTel.Api.Customers.Services;

public class CustomersService(ILogger<CustomersService> logger) : ICustomersService
{
    public async Task<Customer> GetCustomerAsync(CancellationToken cancellationToken = default)
    {
        using var activity = OTel.ActivitySource.StartActivity($"{nameof(CustomersService)}.{nameof(GetCustomerAsync)}");

        // simulate delay
        var delay = Random.Shared.Next(100, 800);
        await Task.Delay(delay, cancellationToken);

        var customer = new Customer();
        customer.CustomerId = Faker.RandomNumber.Next();
        customer.FirstName = Faker.Name.First();
        customer.LastName = Faker.Name.Last();
        customer.Email = Faker.Internet.Email();

        // logger
        logger.LogInformation("Retrieved Customer: ({FirstName} {LastName})", customer.FirstName, customer.LastName);

        // get orders (http://localhost:5179/scalar)
        var order = await GetOrderAsync(cancellationToken);
        if (order is not null)
        {
            customer.Orders.Add(order);
        }

        // metric
        var tagList = new TagList();
        tagList.Add("customer.id", customer.CustomerId);
        tagList.Add("customer.email", customer.Email);

        OTel.Meters.AddGetCustomer(1, tagList);

        // set tags
        activity?.SetTag("customer.id", customer.CustomerId);
        activity?.SetTag("customer.email", customer.Email);

        // event
        var tags = new ActivityTagsCollection();
        tags["customer.id"] = customer.CustomerId;
        tags["customer.email"] = customer.Email;

        activity?.AddEvent(new ActivityEvent("MrJB.OTel.Customers.API.Get", DateTimeOffset.Now, tags));

        return customer;
    }

    private static async Task<Order?> GetOrderAsync(CancellationToken cancellationToken)
    {
        var options = new RestClientOptions("http://localhost:5179/");
        using var client = new RestClient(options);
        var request = new RestRequest("orders");

        return await client.PostAsync<Order>(request, cancellationToken);
    }
}
