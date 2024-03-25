using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace MrJb.NetAspire.OpenTelemetry;

public static class OTel
{
    public static string ServiceName { get; set; } = "MrJb.NetAspire.Console";

    public static string ServiceVersion { get; set; } = "1.0.3";

    public static readonly ActivitySource ConsumerService = new ActivitySource(OTel.ServiceName, OTel.ServiceVersion);

    public static class MetricNames
    {
        public const string BaseName = "mrjb.otel";

        //public const string GetOrders = $"{BaseName}.console.get";

        //public const string SaveOrder = $"{BaseName}.console.save";
    }

    public static class Meters
    {
        public static Meter Meter = new Meter(OTel.ServiceName, OTel.ServiceVersion);

        //private static Counter<int> GetOrders = Meter.CreateCounter<int>(OTel.MetricNames.GetOrders, description: "Tracks when a order is retrieved.");

        //private static Counter<int> SaveOrder = Meter.CreateCounter<int>(OTel.MetricNames.SaveOrder, description: "Tracks when a order is saved.");

        //public static void AddGetOrders(int count = 1) => GetOrders.Add(count);
        //public static void AddGetOrders(int count = 1, TagList tagList = new TagList()) => GetOrders.Add(count, tagList);

        //public static void SaveOrders(int count = 1) => SaveOrder.Add(count);
        //public static void SaveOrders(int count = 1, TagList tagList = new TagList()) => SaveOrder.Add(count, tagList);
    }
}
