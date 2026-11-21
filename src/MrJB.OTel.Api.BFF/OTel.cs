using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace MrJB.OTel.Api.BFF;

internal static class OTel
{
    internal const string ActivitySourceName = "MrJB.OTel.Api.BFF";

    internal static readonly string Version =
        typeof(OTel).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+')[0] ?? "1.0.0";

    internal static readonly ActivitySource ActivitySource = new(ActivitySourceName, Version);

    public static class MetricNames
    {
        public const string BaseName = "mrjb.otel";

        public const string GetCustomerEmail = $"{BaseName}.bff.customer_email.get";
    }

    public static class Meters
    {
        public static readonly Meter Meter = new(ActivitySourceName, Version);

        private static readonly Counter<int> GetCustomerEmail = Meter.CreateCounter<int>(MetricNames.GetCustomerEmail, description: "Tracks when the BFF builds an aggregated customer email.");

        public static void AddGetCustomerEmail(int count = 1) => GetCustomerEmail.Add(count);
        public static void AddGetCustomerEmail(int count, TagList tagList) => GetCustomerEmail.Add(count, tagList);
    }
}
