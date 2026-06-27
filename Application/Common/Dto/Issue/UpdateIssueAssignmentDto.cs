namespace ProjectPlanner.Application.Common.Dto.Issue;

public record UpdateIssueAssignmentDto(
    int? AssigneeId,
    int? ReporterId,
    int? ParentIssueId,
    uint RowVersion
);