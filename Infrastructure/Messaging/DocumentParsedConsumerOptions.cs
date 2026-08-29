namespace ProjectPlanner.Infrastructure.Messaging;

public sealed class DocumentParsedConsumerOptions
{
    public int MaxProcessingAttempts { get; set; } = 3;
    public int RetryBaseDelaySeconds { get; set; } = 5;
    public string DeadLetterTopic { get; set; } = "document.parsed.dlq";
}
