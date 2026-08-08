namespace ProjectPlanner.Application.Common.Dtos.AI;

public class RagSourceDto
{
    public long ChunkId { get; init; }
    public string? DocumentName { get; init; }
    public int? IssueId { get; init; }
    public string Content { get; init; } = null!;
    public double Score { get; init; }
}