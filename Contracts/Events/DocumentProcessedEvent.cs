namespace Contracts.Events;

public record DocumentProcessedEvent
{
    public long AttachmentId { get; init; }
    public int IssueId { get; init; }
    public bool Success { get; init; }
    public int ChunkCount { get; init; }
    public DateTime ProcessedAt { get; init; }
    public string? ErrorMessage { get; init; }
}