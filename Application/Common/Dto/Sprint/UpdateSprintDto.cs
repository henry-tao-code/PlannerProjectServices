using Domain.Entities;
using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dto.Sprint;

public record UpdateSprintDto(
    string Name,
    DateTime? StartDate,
    DateTime? EndDate,
    SprintStatus Status,
    string? Goal);
