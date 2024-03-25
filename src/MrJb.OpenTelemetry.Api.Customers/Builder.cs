using MrJb.OpenTelemetry.Api.Customers;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.DependencyInjection;

public static class Builder
{
    public static IServiceCollection BootstrapApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // open telemetry
        services.ConfigureOpenTelemetry(configuration);

        return services;
    }

    public static IServiceCollection ConfigureOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        // honeycomb
        var honeycombOptions = configuration.GetHoneycombOptions();
        var honeyCombApiKey = configuration["Honeycomb:ApiKey"];

        // open telemetry
        var resource = ResourceBuilder
            .CreateDefault()
            .AddService(OTel.ServiceName)
            .AddTelemetrySdk()
            .AddEnvironmentVariableDetector();

        services.AddOpenTelemetry().WithTracing(builder => builder
            .SetResourceBuilder(resource)
            .AddSource(OTel.ActivitySource.Name)
            .AddHoneycomb(honeycombOptions)
            //.AddCommonInstrumentations()
            //.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            //.AddAspNetCoreInstrumentationWithBaggage()
            .AddOtlpExporter(option =>
            {
                option.Endpoint = new Uri("https://api.honeycomb.io/v1/traces");
                option.Headers = $"x-honeycomb-team={honeyCombApiKey}";
                option.Protocol = OtlpExportProtocol.HttpProtobuf;
            }));

        // register tracer so it can be injected into other components(eg Controllers)
        services.AddSingleton(TracerProvider.Default.GetTracer(honeycombOptions.ServiceName));

        return services;
    }
}
