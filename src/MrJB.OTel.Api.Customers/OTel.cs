using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace MrJB.OTel.Api.Customers;

internal sealed class OTel
{
    /// <summary>
    /// The assembly name.
    /// </summary>
    internal static readonly AssemblyName AssemblyName = typeof(OTel).Assembly.GetName();

    /// <summary>
    /// The activity source name.
    /// </summary>
    internal static readonly string ActivitySourceName = "MrJB.OTel.Api.Customers";

    /// <summary>
    /// The activity source.
    /// </summary>
    internal static readonly ActivitySource ActivitySource = new ActivitySource(ActivitySourceName, GetVersion<OTel>());

    internal static string GetVersion<T>()
    {
        return typeof(T).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion.Split('+')[0];
    }

    public static class MetricNames
    {
        public const string BaseName = "mrjb.otel";

        public const string GetCustomer = $"{BaseName}.customers.get";

        public const string SaveCustomer = $"{BaseName}.customers.save";
    }

    public static class Meters
    {
        public static Meter Meter = new Meter(ActivitySourceName, GetVersion<OTel>());

        private static Counter<int> GetCustomer = Meter.CreateCounter<int>(OTel.MetricNames.GetCustomer, description: "Tracks when a customer is retrieved.");

        private static Counter<int> SaveCustomer = Meter.CreateCounter<int>(OTel.MetricNames.SaveCustomer, description: "Tracks when a customer is saved.");

        public static void AddGetCustomer(int count = 1) => GetCustomer.Add(count);
        public static void AddGetCustomer(int count = 1, TagList tagList = new TagList()) => GetCustomer.Add(count, tagList);

        public static void AddSaveCustomer(int count = 1) => SaveCustomer.Add(count);
        public static void AddSaveCustomer(int count = 1, TagList tagList = new TagList()) => SaveCustomer.Add(count, tagList);
    }
}
