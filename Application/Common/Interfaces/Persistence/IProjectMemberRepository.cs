using Domain.Entities;

namespace ProjectPlanner.Application.Common.Interfaces.Persistence;

public interface IProjectMemberRepository
{
    Task<ProjectMember?> GetAsync(int projectId, int userId, CancellationToken ct);
    Task<IEnumerable<ProjectMember>> GetByProjectIdAsync(int projectId, CancellationToken ct);
    Task AddAsync(ProjectMember member, CancellationToken ct);
    void Remove(ProjectMember member);
}