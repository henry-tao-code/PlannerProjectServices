namespace ProjectPlanner.Application.Common.Interfaces.Messaging;

public interface IKafkaProducer
{
    Task PublishAsync<T>(
        string topic,
        T message,
        CancellationToken cancellationToken = default);
}