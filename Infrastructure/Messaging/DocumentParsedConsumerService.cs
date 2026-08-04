using Confluent.Kafka;
using Contracts.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjectPlanner.Application.Services;
using System.Text.Json;

namespace ProjectPlanner.Infrastructure.Messaging;

public sealed class DocumentParsedConsumerService(
    IConfiguration configuration,
    IServiceScopeFactory scopeFactory,
    ILogger<DocumentParsedConsumerService> logger)
    : BackgroundService
{
    private const string DefaultTopic = "document.parsed";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "kafka:29092";
        var topic = configuration["Kafka:Topics:DocumentParsed"] ?? DefaultTopic;

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = "planner-document-processor",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // 3. FIX: Changed Key type to 'Ignore' to safely handle messages without keys
        using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();

        consumer.Subscribe(topic);
        logger.LogInformation("Listening to Kafka topic {Topic}", topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumeResult<Ignore, string>? result = null;
                try
                {
                    // This is a blocking call, which is why Task.Yield() was required above
                    result = consumer.Consume(stoppingToken);

                    if (string.IsNullOrWhiteSpace(result.Message?.Value))
                        continue;

                    var parsedEvent = JsonSerializer.Deserialize<DocumentParsedEvent>(
                        result.Message.Value, jsonOptions);

                    if (parsedEvent == null)
                    {
                        logger.LogWarning("Invalid document.parsed event at offset {Offset}", result.Offset);
                        consumer.Commit(result); // Commit to avoid poison pill
                        continue;
                    }

                    using var scope = scopeFactory.CreateScope();
                    var processor = scope.ServiceProvider.GetRequiredService<IDocumentParsedProcessor>();

                    await processor.ProcessAsync(parsedEvent, stoppingToken);

                    // Commit only after successful processing
                    consumer.Commit(result);
                    logger.LogInformation("Processed attachment {AttachmentId}", parsedEvent.AttachmentId);
                }
                catch (JsonException ex)
                {
                    logger.LogError(ex, "Invalid JSON message. Skipping message at offset {Offset}", result?.Offset);
                    // 2a. FIX: Commit bad JSON so it doesn't become a poison pill on app restart
                    if (result != null)
                    {
                        consumer.Commit(result);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Transient error processing document.parsed event");

                    // 2b. FIX: Do NOT commit here. Pause before retrying to prevent rapid fail-loops 
                    // if the database or external API is down.
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

                    // Note: Since we didn't commit, Kafka will re-deliver this message if the consumer restarts.
                    // If you want it to retry immediately without restarting, you'd need a Polly retry policy 
                    // inside `processor.ProcessAsync`.
                }
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Document consumer stopping");
        }
        finally
        {
            consumer.Close();
        }
    }
}