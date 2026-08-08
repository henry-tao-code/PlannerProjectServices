namespace Contracts.Dtos;

public record EmbeddingResponseDto
{
    public IReadOnlyList<float[]> Embeddings { get; init; } = [];
    public int TotalTokens { get; init; }
}