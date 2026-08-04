using Confluent.Kafka;
using System.Text.Json;

namespace ProjectPlanner.Infrastructure.Messaging;

public class KafkaJsonDeserializer<T> : IDeserializer<T>
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull || data.IsEmpty)
        {
            return default!;
        }

        return JsonSerializer.Deserialize<T>(data, Options)!;
    }
}