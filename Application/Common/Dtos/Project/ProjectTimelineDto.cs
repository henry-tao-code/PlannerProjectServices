using ProjectPlanner.Application.Common.Dtos.Shared;

namespace ProjectPlanner.Application.Common.Dtos.Project;

public record ProjectTimelineDto(
    int ProjectId,
    IEnumerable<TimelineItemDto> Sprints,
    IEnumerable<TimelineItemDto> Epics);
