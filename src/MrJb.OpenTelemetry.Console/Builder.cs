using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MrJb.OpenTelemetry.Console.Subscribers;
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

        services.AddTransient<ISubscriber, AzureServiceBusSubscriber>();

        return services;
    }

    public static IServiceCollection ConfigureOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        //// honeycomb
        //var honeycombOptions = configuration.GetHoneycombOptions();
        //var honeyCombApiKey = configuration["Honeycomb:ApiKey"];

        //// open telemetry
        //var resource = ResourceBuilder
        //    .CreateDefault()
        //    .AddService(OTel.ServiceName)
        //    .AddTelemetrySdk()
        //    .AddEnvironmentVariableDetector();

        //services.AddOpenTelemetry().WithTracing(builder => builder
        //    .SetResourceBuilder(resource)
        //    //.AddHoneycomb(honeycombOptions)
        //    .AddCommonInstrumentations()
        //    .AddAspNetCoreInstrumentation()
        //    .AddHttpClientInstrumentation()
        //    .AddAspNetCoreInstrumentationWithBaggage()
        //    .AddOtlpExporter(option =>
        //    {
        //        option.Endpoint = new Uri("https://api.honeycomb.io/v1/traces");
        //        option.Headers = $"x-honeycomb-team={honeyCombApiKey}";
        //        option.Protocol = OtlpExportProtocol.HttpProtobuf;
        //    }));

        //// register tracer so it can be injected into other components (eg Controllers)
        //services.AddSingleton(TracerProvider.Default.GetTracer(honeycombOptions.ServiceName));

        return services;
    }



    //public static WebApplicationBuilder ConfigureOpenTelemetry(this WebApplicationBuilder builder)
    //{
    //    // define attributes for your application
    //    var resourceBuilder = ResourceBuilder.CreateDefault()
    //        .AddService(OTel.ApplicationName, serviceVersion: "1.0.0")
    //        .AddTelemetrySdk()
    //        .AddAttributes(new Dictionary<string, object>
    //        {
    //            ["host.name"] = Environment.MachineName,
    //            ["os.description"] = RuntimeInformation.OSDescription,
    //            ["deployment.environment"] = builder.Environment.EnvironmentName.ToLowerInvariant()
    //        });
    //    //.AddConsoleExporter()

    //    // 
    //    builder.Services
    //        .AddOpenTelemetry()
    //        .WithTracing(tb => tb
    //            .AddSource(OTel.Application.Name)
    //            .AddSource(IdentityServerConstants.Tracing.Basic)
    //            .AddSource(IdentityServerConstants.Tracing.Cache)
    //            .AddSource(IdentityServerConstants.Tracing.Services)
    //            .AddSource(IdentityServerConstants.Tracing.Stores)
    //            .AddSource(IdentityServerConstants.Tracing.Validation)
    //            .ConfigureResource(r => r.AddService(OTel.ApplicationName))
    //        //.AddAzureMonitorTraceExporter(options => options.ConnectionString = appInsightsConnectionString)
    //        )
    //        .WithMetrics(mb => mb.ConfigureResource(r => r.AddService(OTel.ApplicationName)))
    //        //.AddAzureMonitorMetricExporter(options => options.ConnectionString = appInsightsConnectionString)
    //        //.AddConsoleExporter()
    //        ;

    //    return builder;
    //}
}
