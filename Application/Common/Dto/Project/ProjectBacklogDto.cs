using ProjectPlanner.Application.Common.Dto.Issue;

namespace ProjectPlanner.Application.Common.Dto.Project;

public record ProjectBacklogDto(
    int ProjectId,
    IEnumerable<BacklogIssueDto> UnassignedIssues,
    IEnumerable<BacklogSprintDto> FutureSprints);
