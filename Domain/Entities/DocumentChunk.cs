namespace Domain.Entities;

public class DocumentChunk
{
    public long Id { get; set; }
    public long AttachmentId { get; set; }
    public IssueAttachment Attachment { get; set; } = null!;
    public int ChunkIndex { get; set; }
    public string Content { get; set; } = null!;
    public float[]? Embedding { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
