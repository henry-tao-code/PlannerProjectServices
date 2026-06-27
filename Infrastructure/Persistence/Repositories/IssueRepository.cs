using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ProjectPlanner.Application.Common.Interfaces.Persistence;
using ProjectPlanner.Infrastructure.Persistence;

namespace ProjectPlanner.Infrastructure.Persistence.Repositories;

public class IssueRepository(ProjectPlannerDbContext dbContext) : IIssueRepository
{
    public async Task<Issue?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await dbContext.Issues
            .Include(i => i.Assignee)
            .Include(i => i.Reporter)
            .Include(i => i.Sprint)
            .FirstOrDefaultAsync(i => i.Id == id, ct);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return await dbContext.Issues.AnyAsync(i => i.Id == id, ct);
    }

    public async Task<IEnumerable<Issue>> GetByProjectIdAsync(int projectId, CancellationToken ct = default)
    {
        return await dbContext.Issues
            .Include(i => i.Assignee)
            .Where(i => i.ProjectId == projectId)
            .ToListAsync(ct);
    }

    public async Task<int> GetCountByProjectIdAsync(int projectId, CancellationToken ct = default)
    {
        return await dbContext.Issues
            .CountAsync(i => i.ProjectId == projectId, ct);
    }

    public async Task AddAsync(Issue issue, CancellationToken ct = default)
    {
        await dbContext.Issues.AddAsync(issue, ct);
    }

    public void Remove(Issue issue)
    {
        dbContext.Issues.Remove(issue);
    }
}