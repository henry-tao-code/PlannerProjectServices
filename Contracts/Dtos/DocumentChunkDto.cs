namespace Contracts.Dtos;

public sealed record DocumentChunkDto
{
    public int Index { get; init; }
    public string Content { get; init; } = string.Empty;
    public int TokenCount { get; init; }
    public float[]? Embedding { get; init; }
}
