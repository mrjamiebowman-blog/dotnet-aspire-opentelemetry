using MrJB.OTel.Domain.Models;

namespace MrJB.OTel.Api.Orders.Services;

public interface IOrdersService
{
    Task<Order> GetOrderAsync(CancellationToken cancellationToken = default);
}
