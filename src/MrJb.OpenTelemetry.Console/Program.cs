using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MrJb.OpenTelemetry.Console;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Configuration;
using System.Reflection;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
    .CreateLogger();

// starting identity server
Log.Information("Starting Console App...");

using IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog((ctx, lc) => lc
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration))
    .ConfigureAppConfiguration((hostContext, configuration) =>
    {
        // configuration
        configuration
            .Sources.Clear();

        // env
        var env = hostContext.HostingEnvironment;

        configuration
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddJsonFile("appsettings.mrjb.json", optional: true, reloadOnChange: true);

        var settings = configuration
            .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
            .Build();
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.BootstrapApplication(hostContext.Configuration);
        services.AddHostedService<HostedService>();
    }).Build();

await host.RunAsync();