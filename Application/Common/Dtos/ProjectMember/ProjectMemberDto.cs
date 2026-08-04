using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dtos.ProjectMember;

public record ProjectMemberDto(
    int ProjectId,
    int UserId,
    string Username,
    ProjectRole Role,
    DateTime JoinedAt);