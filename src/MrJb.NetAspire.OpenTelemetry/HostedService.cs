using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MrJb.OpenTelemetry.Console.Subscribers;

namespace MrJb.OpenTelemetry.Console;

public class HostedService : IHostedService
{
    private readonly ILogger<HostedService> _logger;

    private readonly IHostApplicationLifetime _appLifetime;

    private readonly ISubscriber _subscriber;

    public HostedService(ILogger<HostedService> logger, IHostApplicationLifetime appLifetime, ISubscriber subscriber)
    {
        _logger = logger;
        _appLifetime = appLifetime;
        _subscriber = subscriber;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // start service
        return _subscriber.StartConsumerAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _subscriber.StopConsumerAsync(cancellationToken);
        _appLifetime.StopApplication();
        return Task.CompletedTask;
    }
}

