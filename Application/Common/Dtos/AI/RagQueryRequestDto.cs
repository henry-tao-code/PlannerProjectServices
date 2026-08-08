namespace ProjectPlanner.Application.Common.Dtos.AI;

public class RagQueryRequestDto
{
    public string Query { get; init; } = null!;
    public int? ProjectId { get; init; }
}