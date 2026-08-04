namespace ProjectPlanner.Application.Common.Dtos.Project;

public record ProjectSettingsDto(
    int ProjectId,
    string Name,
    string? Description,
    bool AllowPublicVisibility,
    IEnumerable<ProjectMemberPermissionDto> AccessControlList);
