namespace ProjectPlanner.Application.Common.Dto.Issue;

public record UpdateIssueTimeTrackingDto(
    int? OriginalEstimateMinutes,
    int? TimeSpentMinutes,
    int? TimeRemainingMinutes,
    uint RowVersion
);