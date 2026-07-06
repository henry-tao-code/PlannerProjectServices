using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dto.ProjectMember;

public record ProjectMemberDto(
    int ProjectId,
    int UserId,
    string Username,
    ProjectRole Role,
    DateTime JoinedAt);