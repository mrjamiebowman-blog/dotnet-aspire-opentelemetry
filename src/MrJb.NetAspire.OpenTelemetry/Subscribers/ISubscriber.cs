namespace MrJb.OpenTelemetry.Console.Subscribers;

public interface ISubscriber
{
    Task StartConsumerAsync(CancellationToken token);

    Task StopConsumerAsync(CancellationToken token);
}

