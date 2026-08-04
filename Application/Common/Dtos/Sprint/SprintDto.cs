using Domain.Enums;
using ProjectPlanner.Application.Common.Dtos.Issue;

namespace ProjectPlanner.Application.Common.Dtos.Sprint;

public record SprintDto(
    int Id,
    int ProjectId,
    string Name,
    DateTime? StartDate,
    DateTime? EndDate,
    SprintStatus Status,
    string? Goal,
    IEnumerable<IssueSummaryDto> Issues);
