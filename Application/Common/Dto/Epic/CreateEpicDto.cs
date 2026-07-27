namespace ProjectPlanner.Application.Common.Dto.Epic;

public record CreateEpicDto(
    string Title,
    string Summary,
    string? Description,
    int ProjectId,
    int? AssigneeId,
    DateTime? StartDate,
    DateTime? DueDate
);