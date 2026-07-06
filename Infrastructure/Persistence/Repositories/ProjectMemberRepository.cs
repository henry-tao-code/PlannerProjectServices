using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Infrastructure.Persistence.Repositories;

public class ProjectMemberRepository(ProjectPlannerDbContext context)
    : IProjectMemberRepository
{
    public async Task<ProjectMember?> GetAsync(
        int projectId,
        int userId,
        CancellationToken ct = default)
    {
        return await context.ProjectMembers
            .FirstOrDefaultAsync(
                pm => pm.ProjectId == projectId &&
                      pm.UserId == userId,
                ct);
    }

    public async Task<IEnumerable<ProjectMember>> GetByProjectIdAsync(
        int projectId,
        CancellationToken ct = default)
    {
        return await context.ProjectMembers
            .Where(pm => pm.ProjectId == projectId)
            .Include(pm => pm.User)
            .ToListAsync(ct);
    }

    public async Task AddAsync(
        ProjectMember member,
        CancellationToken ct = default)
    {
        await context.ProjectMembers.AddAsync(member, ct);
    }

    public void Remove(ProjectMember member)
    {
        context.ProjectMembers.Remove(member);
    }
}