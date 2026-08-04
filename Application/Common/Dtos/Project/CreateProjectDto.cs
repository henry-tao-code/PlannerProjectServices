namespace ProjectPlanner.Application.Common.Dtos.Project;

public record CreateProjectDto(
    string Name,
    string Key,
    int UserId,
    string? Description);
