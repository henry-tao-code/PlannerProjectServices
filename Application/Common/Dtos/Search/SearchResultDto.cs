namespace ProjectPlanner.Application.Common.Dtos.Search;

public class SearchResultDto
{
    public long EntityId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public int ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int? IssueId { get; set; }
    public double Score { get; set; }
}
