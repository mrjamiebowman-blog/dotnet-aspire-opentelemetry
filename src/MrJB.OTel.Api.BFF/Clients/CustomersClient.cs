using MrJB.OTel.Domain.Models;

namespace MrJB.OTel.Api.BFF.Clients;

/// <summary>
/// Typed HttpClient for MrJB.OTel.Api.Customers. The base address is resolved through
/// service discovery ("api-customers"), so Aspire wires it up and the W3C trace context
/// flows from the BFF into the Customers API automatically.
/// </summary>
public class CustomersClient(HttpClient httpClient)
{
    public async Task<Customer?> GetCustomerAsync(CancellationToken cancellationToken = default)
    {
        // the customers api exposes its "get customer" operation as POST /customers
        using var response = await httpClient.PostAsync("customers", content: null, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Customer>(cancellationToken);
    }
}
