using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dtos.Issue;

public record BacklogIssueDto(
    int Id,
    string IssueKey,
    string Title,
    IssuePriority Priority,
    IssueType Type);