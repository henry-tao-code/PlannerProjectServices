using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dto.Issue;

public record BoardIssueDto(
    int Id,
    string IssueKey,
    string Title,
    string? AssigneeName,
    string? AssigneeAvatarUrl,
    IssuePriority Priority);