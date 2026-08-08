namespace Contracts.Dtos;

public record EmbeddingRequestDto
{
    public IReadOnlyList<string> Inputs { get; init; } = [];
    public string? Model { get; init; }
    public bool Truncate { get; init; } = true;
}