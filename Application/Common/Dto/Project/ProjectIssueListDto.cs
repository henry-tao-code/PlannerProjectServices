using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dto.Project;

public record ProjectIssueListDto(
    int Id,
    string IssueKey,
    string Title,
    IssueStatus Status,
    IssuePriority Priority,
    string Type,
    string AssigneeName,
    DateTime UpdatedAt);