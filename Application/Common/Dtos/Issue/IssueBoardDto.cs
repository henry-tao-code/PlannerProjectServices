using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dtos.Issue;

public class IssueBoardDto
{
    public int Id { get; set; }
    public string IssueKey { get; set; } = null!;
    public string Title { get; set; } = null!;
    public IssueStatus Status { get; set; }
    public IssuePriority Priority { get; set; }
    public int? AssigneeId { get; set; }
}