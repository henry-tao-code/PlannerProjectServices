using Confluent.Kafka;
using Contracts.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Domain.Enums;
using ProjectPlanner.Application.Common.Interfaces.Persistence;
using ProjectPlanner.Application.Services;
using System.Text.Json;

namespace ProjectPlanner.Infrastructure.Messaging;

public class DocumentParsedConsumerService(
    IConfiguration configuration,
    IServiceScopeFactory scopeFactory,
    IProducer<string, string> producer,
    IOptions<DocumentParsedConsumerOptions> consumerOptions,
    ILogger<DocumentParsedConsumerService> logger)
    : BackgroundService
{
    private const string DefaultTopic = "document.parsed";
    private readonly DocumentParsedConsumerOptions _options = consumerOptions.Value;

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
        // logger.LogInformation("Listening to Kafka topic {Topic}", topic);

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

                    Exception? lastException = null;
                    var maxAttempts = Math.Max(1, _options.MaxProcessingAttempts);

                    for (var attempt = 1; attempt <= maxAttempts; attempt++)
                    {
                        try
                        {
                            using var scope = scopeFactory.CreateScope();
                            var processor = scope.ServiceProvider.GetRequiredService<IDocumentParsedProcessor>();

                            logger.LogInformation(
                                "Processing attachment {AttachmentId} at {Topic}[{Partition}]@{Offset}, attempt {Attempt}/{MaxAttempts}",
                                parsedEvent.AttachmentId, result.Topic, result.Partition.Value, result.Offset.Value,
                                attempt, maxAttempts);

                            await processor.ProcessAsync(parsedEvent, stoppingToken);
                            lastException = null;
                            break;
                        }
                        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            lastException = ex;
                            if (!IsTransient(ex) || attempt == maxAttempts)
                            {
                                break;
                            }

                            var delay = TimeSpan.FromSeconds(
                                Math.Min(_options.RetryBaseDelaySeconds * Math.Pow(2, attempt - 1), 60));
                            logger.LogWarning(
                                ex,
                                "Transient failure for attachment {AttachmentId}; retrying in {DelaySeconds} seconds",
                                parsedEvent.AttachmentId, delay.TotalSeconds);
                            await Task.Delay(delay, stoppingToken);
                        }
                    }

                    if (lastException is not null)
                    {
                        await PublishDeadLetterAsync(result, parsedEvent, lastException, stoppingToken);
                        await MarkAttachmentFailedAsync(parsedEvent.AttachmentId, lastException, stoppingToken);
                    }

                    // Success and dead-letter publication are both terminal outcomes for this offset.
                    consumer.Commit(result);
                    // logger.LogInformation("Processed attachment {AttachmentId}", parsedEvent.AttachmentId);
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
                    logger.LogError(ex, "Unexpected document.parsed consumer failure");

                    if (result is not null)
                    {
                        consumer.Seek(result.TopicPartitionOffset);
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
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

    private static bool IsTransient(Exception exception) => exception switch
    {
        TaskCanceledException => true,
        TimeoutException => true,
        HttpRequestException { StatusCode: null } => true,
        HttpRequestException { StatusCode: var status } when
            status is System.Net.HttpStatusCode.RequestTimeout or
                System.Net.HttpStatusCode.TooManyRequests || (int)status >= 500 => true,
        _ => false
    };

    private async Task PublishDeadLetterAsync(
        ConsumeResult<Ignore, string> result,
        DocumentParsedEvent parsedEvent,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var envelope = JsonSerializer.Serialize(new
        {
            parsedEvent,
            source = new
            {
                result.Topic,
                Partition = result.Partition.Value,
                Offset = result.Offset.Value
            },
            error = new
            {
                Type = exception.GetType().FullName,
                exception.Message
            },
            failedAt = DateTimeOffset.UtcNow
        });

        await producer.ProduceAsync(
            _options.DeadLetterTopic,
            new Message<string, string>
            {
                Key = parsedEvent.AttachmentId.ToString(),
                Value = envelope
            },
            cancellationToken);

        logger.LogError(
            exception,
            "Attachment {AttachmentId} exhausted processing attempts and was published to {DeadLetterTopic}",
            parsedEvent.AttachmentId, _options.DeadLetterTopic);
    }

    private async Task MarkAttachmentFailedAsync(
        long attachmentId,
        Exception exception,
        CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IIssueAttachmentRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var attachment = await repository.GetByIdAsync(attachmentId, cancellationToken);
        if (attachment is null)
        {
            return;
        }

        attachment.Status = AttachmentStatus.Failed;
        attachment.ProcessingError = exception.Message.Length <= 2000
            ? exception.Message
            : exception.Message[..2000];
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
