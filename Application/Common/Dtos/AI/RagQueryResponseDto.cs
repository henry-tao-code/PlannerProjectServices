using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dtos.AI;

public class RagQueryResponseDto
{
    public string Answer { get; init; } = null!;
    public SearchIntent Intent { get; init; }
    public IReadOnlyList<RagSourceDto> Sources { get; init; } = [];
}