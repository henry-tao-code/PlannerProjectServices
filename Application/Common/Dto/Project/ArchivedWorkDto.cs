using ProjectPlanner.Application.Common.Dto.Shared;

namespace ProjectPlanner.Application.Common.Dto.Project;

public record ArchivedWorkDto(
    int ProjectId,
    IEnumerable<ProjectIssueListDto> ArchivedIssues,
    IEnumerable<TimelineItemDto> ArchivedSprints);
