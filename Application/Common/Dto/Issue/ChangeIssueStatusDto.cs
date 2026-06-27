using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dto.Issue;

public class ChangeIssueStatusDto
{
    public IssueStatus Status { get; set; }
}