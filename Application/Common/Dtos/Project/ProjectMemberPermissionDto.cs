using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dtos.Project;

public record ProjectMemberPermissionDto(
    int UserId,
    string Username,
    string Email,
    ProjectRole Role);