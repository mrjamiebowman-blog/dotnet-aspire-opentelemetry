namespace MrJB.OTel.Console.Subscribers;

public interface ISubscriber
{
    Task StartConsumerAsync(CancellationToken token);

    Task StopConsumerAsync(CancellationToken token);
}

