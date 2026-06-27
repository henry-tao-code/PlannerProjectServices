using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dto.Issue;

public record BacklogIssueDto(
    int Id,
    string IssueKey,
    string Title,
    IssuePriority Priority,
    IssueType Type);