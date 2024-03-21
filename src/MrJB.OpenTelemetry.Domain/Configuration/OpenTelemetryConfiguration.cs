namespace MrJB.OpenTelemetry.Domain.Configuration;

public class OpenTelemetryConfiguration
{
    public const string Position = "OpenTelemetry";

    public HoneyCombConfiguration HoneyComb { get; set; } = new HoneyCombConfiguration();


}
