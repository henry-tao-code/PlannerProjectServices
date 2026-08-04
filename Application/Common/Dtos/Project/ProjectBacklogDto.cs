using ProjectPlanner.Application.Common.Dtos.Issue;

namespace ProjectPlanner.Application.Common.Dtos.Project;

public record ProjectBacklogDto(
    int ProjectId,
    IEnumerable<BacklogIssueDto> UnassignedIssues,
    IEnumerable<BacklogSprintDto> FutureSprints);
