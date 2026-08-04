using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dtos.Sprint;

public record UpdateSprintDto(
    string Name,
    DateTime? StartDate,
    DateTime? EndDate,
    SprintStatus Status,
    string? Goal);
