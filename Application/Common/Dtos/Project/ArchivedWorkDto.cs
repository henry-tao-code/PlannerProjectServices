using ProjectPlanner.Application.Common.Dtos.Shared;

namespace ProjectPlanner.Application.Common.Dtos.Project;

public record ArchivedWorkDto(
    int ProjectId,
    IEnumerable<ProjectIssueListDto> ArchivedIssues,
    IEnumerable<TimelineItemDto> ArchivedSprints);
