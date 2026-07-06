using Domain.Enums;
using ProjectPlanner.Application.Common.Dto.ProjectMember;
namespace ProjectPlanner.Application.Services;

public interface IProjectMemberService
{
    Task<ProjectMemberDto> AddMemberAsync(
        int projectId,
        int userId,
        ProjectRole role,
        int addedByUserId,
        CancellationToken ct = default);
    Task<IEnumerable<ProjectMemberDto>> GetMembersAsync(
        int projectId,
        CancellationToken ct = default);
    Task RemoveMemberAsync(
        int projectId,
        int userId,
        CancellationToken ct = default);
    Task ChangeRoleAsync(
        int projectId,
        int userId,
        ProjectRole role,
        int updatedByUserId,
        CancellationToken ct = default);
}