using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Infrastructure.Persistence.Repositories;

public class EpicRepository(ProjectPlannerDbContext context) : IEpicRepository
{
    public async Task<Epic?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Epics
            .Include(e => e.Assignee) // Eagerly load assignee for UI username resolution
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);
    }

    public async Task<Epic?> GetByIdWithIssuesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Epics
            .Include(e => e.Assignee)
            .Include(e => e.Issues.Where(i => !i.IsDeleted)) // Filter out deleted issues inside the epic
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<Epic>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default)
    {
        return await context.Epics
            .AsNoTracking() // Performance optimization since this is primarily for lists/boards
            .Include(e => e.Assignee)
            .Include(e => e.Issues) // Needed if your EpicSummaryDto calculates TotalIssuesCount
            .Where(e => e.ProjectId == projectId && !e.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Epic epic, CancellationToken cancellationToken = default)
    {
        await context.Epics.AddAsync(epic, cancellationToken);
    }

    public void Update(Epic epic)
    {
        context.Epics.Update(epic);
    }

    public void Remove(Epic epic)
    {
        context.Epics.Remove(epic);
    }
}