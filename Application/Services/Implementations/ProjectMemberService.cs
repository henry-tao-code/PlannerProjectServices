using Domain.Entities;
using Domain.Enums;
using ProjectPlanner.Application.Common.Dto.ProjectMember;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Application.Services.Implementations;

public class ProjectMemberService(
    IProjectMemberRepository projectMemberRepository,
    IUserRepository userRepository,
    IProjectRepository projectRepository,
    IUnitOfWork unitOfWork)
    : IProjectMemberService
{
    public async Task<ProjectMemberDto> AddMemberAsync(
        int projectId,
        int userId,
        ProjectRole role,
        int addedByUserId,
        CancellationToken ct = default)
    {
        var project = await projectRepository.GetByIdAsync(projectId, ct)
            ?? throw new KeyNotFoundException("Project not found.");

        var user = await userRepository.GetByIdAsync(userId, ct)
            ?? throw new KeyNotFoundException("User not found.");

        var existing = await projectMemberRepository.GetAsync(projectId, userId, ct);

        if (existing is not null)
            throw new InvalidOperationException("User is already a project member.");

        var member = ProjectMember.Create(
            projectId,
            userId,
            role,
            addedByUserId);

        await projectMemberRepository.AddAsync(member, ct);

        await unitOfWork.SaveChangesAsync(ct);

        return new ProjectMemberDto
        (
            member.ProjectId,
            member.UserId,
            user.Username,
            member.Role,
            member.JoinedAt
        );
    }

    public async Task<IEnumerable<ProjectMemberDto>> GetMembersAsync(
        int projectId,
        CancellationToken ct = default)
    {
        var members = await projectMemberRepository.GetByProjectIdAsync(projectId, ct);

        return members.Select(m => new ProjectMemberDto(
            m.ProjectId,
            m.UserId,
            m.User.Username,
            m.Role,
            m.JoinedAt
        ));
    }

    public async Task ChangeRoleAsync(
        int projectId,
        int userId,
        ProjectRole role,
        int updatedByUserId,
        CancellationToken ct = default)
    {
        var member = await projectMemberRepository.GetAsync(projectId, userId, ct)
            ?? throw new KeyNotFoundException("Project member not found.");

        member.ChangeRole(role, updatedByUserId);

        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task RemoveMemberAsync(
        int projectId,
        int userId,
        CancellationToken ct = default)
    {
        var member = await projectMemberRepository.GetAsync(projectId, userId, ct)
            ?? throw new KeyNotFoundException("Project member not found.");

        projectMemberRepository.Remove(member);

        await unitOfWork.SaveChangesAsync(ct);
    }
}