using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dto.ProjectMember;

public record AddProjectMemberDto(
    int UserId,
    ProjectRole Role);