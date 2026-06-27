using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dto.Issue;

public record UpdateIssueCoreDto(
    string? Title,
    string? Description,
    IssueType? Type,
    IssueStatus? Status,
    IssuePriority? Priority,
    int? StoryPoints,
    DateTime? StartDate,
    DateTime? DueDate,
    uint RowVersion
);