using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dtos.Issue;

public record IssueSummaryDto(
    int Id,
    string IssueKey,
    string Title,
    string? EpicName,
    IssueStatus Status,
    IssuePriority Priority,
    string? AssigneeName,
    int StoryPoints
);