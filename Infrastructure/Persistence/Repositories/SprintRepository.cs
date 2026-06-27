using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Infrastructure.Persistence.Repositories;

public class SprintRepository(ProjectPlannerDbContext context) : ISprintRepository
{
    public async Task<Sprint?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await context.Sprints
            .Include(s => s.Issues)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Sprint>> GetByProjectIdAsync(
        int projectId,
        CancellationToken cancellationToken = default)
    {
        return await context.Sprints
            .AsNoTracking()
            .Where(s => s.ProjectId == projectId)
            .OrderByDescending(s => s.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Sprint?> GetActiveSprintByProjectIdAsync(
        int projectId,
        CancellationToken cancellationToken = default)
    {
        return await context.Sprints
            .Include(s => s.Issues)
            .FirstOrDefaultAsync(
                s => s.ProjectId == projectId &&
                     s.Status == SprintStatus.Active,
                cancellationToken);
    }

    public async Task<Sprint?> GetBacklogSprintByProjectIdAsync(
        int projectId,
        CancellationToken cancellationToken = default)
    {
        return await context.Sprints
            .FirstOrDefaultAsync(
                s => s.ProjectId == projectId &&
                     s.Status == SprintStatus.Backlog,
                cancellationToken);
    }

    public async Task AddAsync(
        Sprint sprint,
        CancellationToken cancellationToken = default)
    {
        await context.Sprints.AddAsync(sprint, cancellationToken);
    }

    public void Delete(Sprint sprint)
    {
        context.Sprints.Remove(sprint);
    }
}