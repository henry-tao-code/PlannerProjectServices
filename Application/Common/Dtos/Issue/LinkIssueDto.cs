using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dtos.Issue;

public class LinkIssueDto
{
    public int TargetIssueId { get; set; }
    public IssueLinkType LinkType { get; set; }
}