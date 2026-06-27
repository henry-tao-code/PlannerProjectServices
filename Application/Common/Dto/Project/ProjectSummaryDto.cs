using ProjectPlanner.Application.Common.Dto.Shared;

namespace ProjectPlanner.Application.Common.Dto.Project;

public record ProjectSummaryDto(
    int ProjectId,
    string Name,
    string OwnerName,
    int ActiveSprintCount,
    int TotalIssuesCount,
    int OpenIssuesCount,
    int DoneIssuesCount,
    IEnumerable<RecentActivityDto> RecentActivities);