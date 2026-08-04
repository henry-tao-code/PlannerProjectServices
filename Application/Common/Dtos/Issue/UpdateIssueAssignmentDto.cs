namespace ProjectPlanner.Application.Common.Dtos.Issue;

public record UpdateIssueAssignmentDto(
    int? AssigneeId,
    int? ReporterId,
    int? ParentIssueId
);