namespace MrJB.OpenTelemetry.Domain.Models;
public class Order
{
    public int? OrderId { get; set; }

    public int? CustomerId { get; set; }

    public decimal? SubTotal { get; set; }

    public decimal? Taxes { get; set; }

    public decimal? Total { get; set; }

    public List<OrderLineItem> Items { get; set; }
}
