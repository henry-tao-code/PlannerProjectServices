using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dto.Issue;

public record IssueSummaryDto(
    int Id,
    string IssueKey,
    string Title,
    IssueStatus Status,
    IssuePriority Priority,
    string? AssigneeName,
    int StoryPoints
);