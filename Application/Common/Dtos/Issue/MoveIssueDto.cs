using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dtos.Issue;

public record MoveIssueDto(
    IssueStatus? TargetStatus,
    int? TargetSprintId
);