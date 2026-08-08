using Contracts.Dtos;

namespace Contracts.Events;

public record DocumentParsedEvent
{
    public int Version { get; init; } = 1;
    public long AttachmentId { get; init; }
    public IReadOnlyList<DocumentChunkDto> Chunks { get; init; } = [];
    public DateTimeOffset ParsedAt { get; init; }
}