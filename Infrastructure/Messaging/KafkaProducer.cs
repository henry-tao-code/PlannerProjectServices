using Confluent.Kafka;
using ProjectPlanner.Application.Common.Interfaces.Messaging;
using System.Text.Json;

namespace ProjectPlanner.Infrastructure.Messaging;

public class KafkaProducer(IProducer<string, string> producer) : IKafkaProducer
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task PublishAsync<T>(
        string topic,
        T message,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(message, JsonOptions);

        await producer.ProduceAsync(
            topic,
            new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = json
            },
            cancellationToken);
    }
}