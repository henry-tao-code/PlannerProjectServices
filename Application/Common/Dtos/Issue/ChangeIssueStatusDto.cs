using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dtos.Issue;

public class ChangeIssueStatusDto
{
    public IssueStatus Status { get; set; }
}