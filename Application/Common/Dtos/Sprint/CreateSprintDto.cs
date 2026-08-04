namespace ProjectPlanner.Application.Common.Dtos.Sprint;

public record CreateSprintDto(
    int ProjectId,
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    string? Goal);