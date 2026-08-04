namespace ProjectPlanner.Application.Common.Dtos.Issue;

public record UpdateIssueTimeTrackingDto(
    int? OriginalEstimateMinutes,
    int? TimeSpentMinutes,
    int? TimeRemainingMinutes
);