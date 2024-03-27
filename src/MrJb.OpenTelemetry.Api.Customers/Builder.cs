using MrJb.OpenTelemetry.Api.Customers;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.DependencyInjection;

public static class Builder
{
    public static IServiceCollection BootstrapApplication(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        // open telemetry
        services.ConfigureOpenTelemetry(configuration, environment);

        return services;
    }

    public static IServiceCollection ConfigureOpenTelemetry(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
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

        services.AddOpenTelemetry()
            // tracing
            .WithTracing(builder =>
            {
                if (environment.IsDevelopment())
                {
                    builder.SetSampler(new AlwaysOnSampler());
                }

                builder.SetResourceBuilder(resource)
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
                       });
            })
            // metrics
            .WithMetrics(builder => builder
                .SetResourceBuilder(resource)
                .AddMeter(OTel.Meters.Meter.Name)
                //.AddMeter("Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel")
                //.AddView("http.server.request.duration", new ExplicitBucketHistogramConfiguration
                //{
                //    Boundaries = new double[] { 0, 0.005, 0.01, 0.025, 0.05,
                //           0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10 }
                //})
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddPrometheusExporter()
                .AddPrometheusHttpListener(options => options.UriPrefixes = new string[] { "http://localhost:9001/" })
            );
        ;

        // use otel exporter otlp endpoint? (.net aspire in a container)
        var otlpEndpoint = configuration.GetValue<string>("DOCKER_OTEL_EXPORTER_OTLP_ENDPOINT");
        var useOtlpExporter = !String.IsNullOrWhiteSpace(otlpEndpoint);

        //if (useOtlpExporter)
        //{
        //    services.Configure<OpenTelemetryLoggerOptions>(logging => logging.AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint)));
        //    services.ConfigureOpenTelemetryMeterProvider(metrics => metrics.AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint)));
        //    services.ConfigureOpenTelemetryTracerProvider(tracing => tracing.AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint)));
        //}

        // register tracer so it can be injected into other components(eg Controllers)
        services.AddSingleton(TracerProvider.Default.GetTracer(honeycombOptions.ServiceName));

        return services;
    }
}
