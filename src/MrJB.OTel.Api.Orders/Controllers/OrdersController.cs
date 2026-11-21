using Microsoft.AspNetCore.Mvc;
using MrJB.OTel.Api.Orders.Services;
using MrJB.OTel.Domain.Models;
using System.Diagnostics;

namespace MrJB.OTel.Api.Orders.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController(IOrdersService ordersService) : ControllerBase
{
    [HttpPost(Name = nameof(GetOrders))]
    public async Task<Order> GetOrders(CancellationToken cancellationToken)
    {
        using var activity = OTel.ActivitySource.StartActivity($"{nameof(OrdersController)}.{nameof(GetOrders)}");

        try
        {
            var order = await ordersService.GetOrderAsync(cancellationToken);

            activity?.SetTag("order.id", order.OrderId);
            activity?.SetStatus(ActivityStatusCode.Ok);

            return order;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddException(ex);
            throw new Exception("Unable to load orders.");
        }
    }
}
