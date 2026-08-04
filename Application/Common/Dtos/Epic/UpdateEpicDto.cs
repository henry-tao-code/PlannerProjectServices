using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dtos.Epic;

public record UpdateEpicDto(
    string Name,
    string Summary,
    string? Description,
    EpicStatus Status,
    int? AssigneeId,
    DateTime? StartDate,
    DateTime? DueDate
);