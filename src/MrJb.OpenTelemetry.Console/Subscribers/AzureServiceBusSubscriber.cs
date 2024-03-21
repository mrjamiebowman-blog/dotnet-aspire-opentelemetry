using Microsoft.Extensions.Logging;

namespace MrJb.OpenTelemetry.Console.Subscribers;

public class AzureServiceBusSubscriber : ISubscriber
{
    // logging
    private readonly ILogger<AzureServiceBusSubscriber> _logger;

    public AzureServiceBusSubscriber(ILogger<AzureServiceBusSubscriber> logger)
    {
        _logger = logger;
    }

    public async Task StartConsumerAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            // do work!

            await Task.Delay(1000);
        }
    }

    public Task StopConsumerAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
