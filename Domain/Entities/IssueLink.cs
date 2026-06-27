using Domain.Entities;
using Domain.Enums;

public class IssueLink
{
    public int Id { get; set; }
    public int SourceIssueId { get; set; }
    public Issue SourceIssue { get; set; } = null!;
    public int TargetIssueId { get; set; }
    public Issue TargetIssue { get; set; } = null!;
    public IssueLinkType LinkType { get; set; }
}

