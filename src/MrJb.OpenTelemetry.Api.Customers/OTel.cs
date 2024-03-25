using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace MrJb.OpenTelemetry.Api.Customers;

public static class OTel
{
    public static string ServiceName { get; set; } = "MrJb.OpenTelemetry.Api.Customers";

    public static string ServiceVersion { get; set; } = "1.0.3";

    public static readonly ActivitySource ActivitySource = new ActivitySource(OTel.ServiceName, OTel.ServiceVersion);

    public static class MetricNames
    {
        public const string BaseName = "mrjb.otel";

        public const string GetOrders = $"{BaseName}.customers.get";

        public const string SaveOrder = $"{BaseName}.customers.save";
    }

    public static class Meters
    {
        public static Meter Meter = new Meter(OTel.ServiceName, OTel.ServiceVersion);

        private static Counter<int> GetCustomer = Meter.CreateCounter<int>(OTel.MetricNames.GetOrders, description: "Tracks when a customer is retrieved.");

        private static Counter<int> SaveCustomer = Meter.CreateCounter<int>(OTel.MetricNames.SaveOrder, description: "Tracks when a customer is saved.");

        public static void AddGetCustomer(int count = 1) => GetCustomer.Add(count);
        public static void AddGetCustomer(int count = 1, TagList tagList = new TagList()) => GetCustomer.Add(count, tagList);

        public static void AddSaveCustomer(int count = 1) => SaveCustomer.Add(count);
        public static void AddSaveCustomer(int count = 1, TagList tagList = new TagList()) => SaveCustomer.Add(count, tagList);
    }
}
