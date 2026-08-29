namespace Domain.Entities;

public class SearchDocument
{
    public int EntityId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string IssueKey { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public string? AssigneeName { get; set; }
    public string? ReporterName { get; set; }
    public int? SprintId { get; set; }
    public string? SprintName { get; set; }
}
