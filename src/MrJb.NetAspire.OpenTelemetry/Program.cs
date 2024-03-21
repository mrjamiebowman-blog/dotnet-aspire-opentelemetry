using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MrJb.OpenTelemetry.Console;
using System.Reflection;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostContext, configuration) =>
    {
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