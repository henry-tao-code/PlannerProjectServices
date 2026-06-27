namespace ProjectPlanner.Application.Common.Dto.Project;

public record CreateProjectDto(
    string Name,
    string Key,
    int UserId,
    string? Description);
