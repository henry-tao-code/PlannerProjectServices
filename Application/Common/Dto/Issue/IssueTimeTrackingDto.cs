namespace ProjectPlanner.Application.Common.Dto.Issue;

public class IssueTimeTrackingDto
{
    public int? OriginalEstimateMinutes { get; set; }
    public int? TimeSpentMinutes { get; set; }
    public int? TimeRemainingMinutes { get; set; }
}