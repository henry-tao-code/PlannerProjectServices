using Domain.Entities;
using Domain.Enums;
using ProjectPlanner.Application.Common.Dto.Issue;

namespace ProjectPlanner.Application.Common.Dto.Sprint;

public record SprintDto(
    int Id,
    int ProjectId,
    string Name,
    DateTime? StartDate,
    DateTime? EndDate,
    SprintStatus Status,
    string? Goal,
    IEnumerable<IssueSummaryDto> Issues);
