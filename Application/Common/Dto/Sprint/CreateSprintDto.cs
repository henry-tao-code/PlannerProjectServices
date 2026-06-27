namespace ProjectPlanner.Application.Common.Dto.Sprint;

public record CreateSprintDto(
    int ProjectId,
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    string? Goal);