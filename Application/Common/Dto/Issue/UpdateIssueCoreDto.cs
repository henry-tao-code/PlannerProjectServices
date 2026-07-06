using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dto.Issue;

public record UpdateIssueCoreDto(
    string? Title,
    string? Description,
    IssueType? Type,
    IssueStatus? Status,
    IssuePriority? Priority,

    int? AssigneeId,
    int? ReporterId,

    int? SprintId,
    int? ParentIssueId,

    int? StoryPoints,

    DateTime? StartDate,
    DateTime? DueDate,

    List<string>? Labels,

    uint RowVersion
);