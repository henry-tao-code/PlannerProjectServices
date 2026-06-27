using Domain.Enums;
using ProjectPlanner.Application.Common.Dto.Issue;

public record UpdateIssueDetailsDto(
    string Title,
    string? Description,

    IssueType IssueType,
    IssueStatus Status,
    IssuePriority Priority,

    int? SprintId,
    int? AssigneeId,
    int? ReporterId,

    DateTime? StartDate,
    DateTime? DueDate,

    int? ParentIssueId,

    int StoryPoints,

    IssueTimeTrackingDto TimeTracking,

    List<string> Labels,

    uint RowVersion
);