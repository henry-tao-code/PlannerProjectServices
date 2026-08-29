using Pgvector;

namespace Domain.Entities;

public class DocumentChunk
{
    public long Id { get; set; }
    public long AttachmentId { get; set; }
    public IssueAttachment Attachment { get; set; } = null!;
    public int ChunkIndex { get; set; }
    public string Content { get; set; } = null!;
    public int TokenCount { get; set; }
    public string ChunkType { get; set; } = "content";
    public int? PageNumber { get; set; }
    public int? TableIndex { get; set; }
    public Vector? Embedding { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
