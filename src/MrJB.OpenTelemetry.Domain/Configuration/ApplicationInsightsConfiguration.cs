namespace MrJB.OpenTelemetry.Domain.Configuration;

public class ApplicationInsightsConfiguration
{
    public const string Position = "ApplicationInsights";

    public string ConnectionString { get; set; }
}
