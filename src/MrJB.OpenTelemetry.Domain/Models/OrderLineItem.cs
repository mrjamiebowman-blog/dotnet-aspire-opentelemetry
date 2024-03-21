namespace MrJB.OpenTelemetry.Domain.Models;

public class OrderLineItem
{
    public int? OrderLineItemId { get; set; }

    public string Name { get; set; }

    public decimal? Price { get; set; }

    public int Quantity { get; set; }
}
