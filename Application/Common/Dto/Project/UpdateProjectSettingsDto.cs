namespace ProjectPlanner.Application.Common.Dto.Project;

public record UpdateProjectSettingsDto(
    string Name,
    string? Description,
    bool AllowPublicVisibility);