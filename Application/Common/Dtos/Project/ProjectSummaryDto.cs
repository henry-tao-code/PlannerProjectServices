using ProjectPlanner.Application.Common.Dtos.Shared;

namespace ProjectPlanner.Application.Common.Dtos.Project;

public record ProjectSummaryDto(
    int ProjectId,
    string Name,
    string OwnerName,
    int ActiveSprintCount,
    int TotalIssuesCount,
    int OpenIssuesCount,
    int DoneIssuesCount,
    IEnumerable<RecentActivityDto> RecentActivities);