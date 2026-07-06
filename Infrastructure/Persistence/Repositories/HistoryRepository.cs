using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Infrastructure.Persistence.Repositories;

public class HistoryRepository(ProjectPlannerDbContext context) : IHistoryRepository
{
    public async Task<IssueHistory?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.History
            .AsNoTracking()
            .Include(h => h.ChangedByUser)
            .FirstOrDefaultAsync(h => h.Id == id, ct);
    }

    public async Task<IEnumerable<IssueHistory>> GetByIssueIdAsync(int issueId, CancellationToken ct = default)
    {
        return await context.History
            .AsNoTracking()
            .Include(h => h.ChangedByUser)
            .Where(h => h.IssueId == issueId)
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<IssueHistory>> GetByProjectIdAsync(
    int projectId,
    int take,
    CancellationToken cancellationToken = default)
    {
        return await context.History
            .AsNoTracking()
            .Include(h => h.ChangedByUser)
            .Where(h => h.Issue.ProjectId == projectId)
            .OrderByDescending(h => h.ChangedAt)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(IssueHistory history, CancellationToken ct = default)
    {
        await context.History.AddAsync(history, ct);
    }
}