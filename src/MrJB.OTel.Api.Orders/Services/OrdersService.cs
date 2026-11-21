using MrJB.OTel.Domain.Models;
using System.Diagnostics;

namespace MrJB.OTel.Api.Orders.Services;

public class OrdersService(ILogger<OrdersService> logger) : IOrdersService
{
    public async Task<Order> GetOrderAsync(CancellationToken cancellationToken = default)
    {
        using var activity = OTel.ActivitySource.StartActivity($"{nameof(OrdersService)}.{nameof(GetOrderAsync)}");

        // simulate delay
        var delay = Random.Shared.Next(1500, 6000);
        await Task.Delay(delay, cancellationToken);

        var order = new Order();
        order.CustomerId = Faker.RandomNumber.Next();
        order.OrderId = Faker.RandomNumber.Next();
        order.Total = Faker.RandomNumber.Next(100, 500);
        order.SubTotal = (order.Total.Value * 0.8M);
        order.Taxes = (order.Total - order.SubTotal);

        // logger
        logger.LogInformation("Retrieved Order: (#{OrderId})", order.OrderId);

        // metric
        var tagList = new TagList();
        tagList.Add("order.id", order.OrderId);
        tagList.Add("customer.id", order.CustomerId);

        OTel.Meters.AddGetOrders(1, tagList);

        // set tags
        activity?.SetTag("order.id", order.OrderId);
        activity?.SetTag("customer.id", order.CustomerId);

        // event
        var tags = new ActivityTagsCollection();
        tags["order.id"] = order.OrderId;
        tags["customer.id"] = order.CustomerId;

        activity?.AddEvent(new ActivityEvent("MrJB.OTel.Orders.API.Get", DateTimeOffset.Now, tags));

        return order;
    }
}
