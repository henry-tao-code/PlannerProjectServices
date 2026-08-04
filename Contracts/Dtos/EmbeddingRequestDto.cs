namespace Contracts.Dtos;

public sealed record EmbeddingRequestDto
{
    public IReadOnlyList<string> Inputs { get; init; } = [];
    public string? Model { get; init; }
    public bool Truncate { get; init; } = true;
}