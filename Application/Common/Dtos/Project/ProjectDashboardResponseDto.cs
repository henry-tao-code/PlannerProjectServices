namespace ProjectPlanner.Application.Common.Dtos.Project;

public record ProjectDashboardResponseDto(
    int Id,
    string Name,
    string? Description,
    string Key,
    int OwnerId,
    DateTime CreatedAt);
