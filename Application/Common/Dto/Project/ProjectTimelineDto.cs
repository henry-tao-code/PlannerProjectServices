using ProjectPlanner.Application.Common.Dto.Shared;

namespace ProjectPlanner.Application.Common.Dto.Project;

public record ProjectTimelineDto(
    int ProjectId,
    IEnumerable<TimelineItemDto> Sprints,
    IEnumerable<TimelineItemDto> Epics);
