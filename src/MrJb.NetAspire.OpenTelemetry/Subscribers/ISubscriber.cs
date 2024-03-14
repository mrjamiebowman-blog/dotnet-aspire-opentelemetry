namespace MrJb.NetAspire.OpenTelemetry.Subscribers;

public interface ISubscriber
{
    Task StartConsumerAsync(CancellationToken token);

    Task StopConsumerAsync(CancellationToken token);
}

