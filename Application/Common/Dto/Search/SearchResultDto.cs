namespace ProjectPlanner.Application.Common.Dto.Search;

public class SearchResultDto
{
    public int EntityId { get; set; }

    public string EntityType { get; set; } = string.Empty;

    public int ProjectId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public double Score { get; set; }
}
