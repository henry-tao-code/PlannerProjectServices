namespace Contracts.Dtos;

public record DocumentChunkDto
{
    public int Index { get; init; }
    public string Content { get; init; } = string.Empty;
    public int TokenCount { get; init; }
    public string Type { get; init; } = "content";
    public int? Page { get; init; }
    public int? TableIndex { get; init; }
    public float[]? Embedding { get; init; }
}
