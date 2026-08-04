namespace Contracts.Dtos;

public sealed record EmbeddingResponseDto
{
    public IReadOnlyList<float[]> Embeddings { get; init; } = [];
    public int TotalTokens { get; init; }
}