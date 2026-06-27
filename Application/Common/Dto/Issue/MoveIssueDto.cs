using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dto.Issue;

public record MoveIssueDto(
    IssueStatus? TargetStatus,
    int? TargetSprintId,
    uint RowVersion
);