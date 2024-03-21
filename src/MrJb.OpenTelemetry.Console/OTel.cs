using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace MrJb.NetAspire.OpenTelemetry;

public static class OTel
{
    public static string ServiceName { get; set; } = "MrJb.NetAspire.Console";

    public static string ServiceVersion { get; set; } = "1.0.3";

    public static readonly ActivitySource ConsumerService = new(ServiceName);

    public static class MetricNames
    {
        public const string test = "mrjb.opentelemetry.";
    }

    public static class Meters
    {
        public static Meter LfcMeter = new Meter(OTel.ServiceName, OTel.ServiceVersion);

        private static Counter<int> Test = LfcMeter.CreateCounter<int>(OTel.MetricNames.test, description: "Tracks when a Test is ran.");

        public static void AddTest() => Test.Add(1);

    }
}
