namespace ProjectPlanner.Application.Common.Dtos.Issue;

public class IssueTimeTrackingDto
{
    public int? OriginalEstimateMinutes { get; set; }
    public int? TimeSpentMinutes { get; set; }
    public int? TimeRemainingMinutes { get; set; }
}