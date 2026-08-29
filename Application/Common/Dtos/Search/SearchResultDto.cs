namespace ProjectPlanner.Application.Common.Dtos.Search;

public record SearchResultDto(
    long EntityId,
    string EntityType,
    string IssueKey,
    string Title,
    string? Content,
    int ProjectId,
    string? ProjectName,
    string? Status,
    string? Priority,
    string? AssigneeName,
    double? Score
)
{
    public string ChunkType { get; init; } = "content";
    public int? PageNumber { get; init; }
    public int? TableIndex { get; init; }
}
