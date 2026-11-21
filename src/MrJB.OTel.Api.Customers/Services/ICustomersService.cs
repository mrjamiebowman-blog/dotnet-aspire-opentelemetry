using MrJB.OTel.Domain.Models;

namespace MrJB.OTel.Api.Customers.Services;

public interface ICustomersService
{
    Task<Customer> GetCustomerAsync(CancellationToken cancellationToken = default);
}
