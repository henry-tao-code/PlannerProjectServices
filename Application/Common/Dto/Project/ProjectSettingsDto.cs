namespace ProjectPlanner.Application.Common.Dto.Project;

public record ProjectSettingsDto(
    int ProjectId,
    string Name,
    string? Description,
    bool AllowPublicVisibility,
    IEnumerable<ProjectMemberPermissionDto> AccessControlList,
    uint RowVersion);
