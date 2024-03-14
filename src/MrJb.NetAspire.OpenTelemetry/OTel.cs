using System.Diagnostics;

namespace MrJb.NetAspire.OpenTelemetry;

public static class OTel
{
    public static string ServiceName { get; set; } = "MrJb.NetAspire.Console";

    public static string ServiceVersion { get; set; } = "1.0.3";

    public static readonly ActivitySource ConsumerService = new(ServiceName);
}
