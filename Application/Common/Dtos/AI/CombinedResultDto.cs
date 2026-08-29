namespace ProjectPlanner.Application.Common.Dtos.AI;

public class CombinedResultDto
{
    public long EntityId { get; init; }
    public string EntityType { get; init; } = string.Empty;
    public int ProjectId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string ChunkType { get; init; } = "content";
    public int? PageNumber { get; init; }
    public int? TableIndex { get; init; }
    public double Score { get; set; }
}
