using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dtos.ProjectMember;

public record AddProjectMemberDto(
    int UserId,
    ProjectRole Role);