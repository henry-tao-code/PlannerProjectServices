namespace ProjectPlanner.Application.Common.Dto.Project;

public record ProjectDashboardResponseDto(
    int Id,
    string Name,
    string? Description,
    string Key,
    int OwnerId,
    DateTime CreatedAt,
    uint RowVersion);
