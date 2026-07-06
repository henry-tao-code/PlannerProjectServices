using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dto.Epic;

public record EpicDto(
    int Id,
    string Name,
    string Summary,
    string? Description,
    EpicStatus Status,
    DateTime? StartDate,
    DateTime? DueDate,
    int ProjectId,
    int? AssigneeId,
    string? AssigneeUsername,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    uint RowVersion
);