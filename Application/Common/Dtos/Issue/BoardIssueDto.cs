using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dtos.Issue;

public record BoardIssueDto(
    int Id,
    string IssueKey,
    string Title,
    string? AssigneeName,
    string? AssigneeAvatarUrl,
    IssuePriority Priority);