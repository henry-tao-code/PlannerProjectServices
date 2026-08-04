namespace ProjectPlanner.Application.Common.Dtos.Project;

public record UpdateProjectSettingsDto(
    string Name,
    string? Description,
    bool AllowPublicVisibility);