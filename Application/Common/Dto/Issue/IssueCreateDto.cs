using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dto.Issue;

public class IssueCreateDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public IssueType IssueType { get; set; } = IssueType.Task;
    public IssuePriority Priority { get; set; } = IssuePriority.Medium;

    public int ProjectId { get; set; }

    public int? SprintId { get; set; }

    public int? AssigneeId { get; set; }
    public int? ReporterId { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }

    public int? ParentIssueId { get; set; }

    public int StoryPoints { get; set; } = 0;

    public IssueTimeTrackingDto? TimeTracking { get; set; }

    public List<string> Labels { get; set; } = [];
}