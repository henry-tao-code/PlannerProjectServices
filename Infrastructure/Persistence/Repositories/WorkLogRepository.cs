using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ProjectPlanner.Application.Common.Interfaces.Persistence;
using ProjectPlanner.Infrastructure.Persistence;

public class WorkLogRepository(ProjectPlannerDbContext context) : IWorkLogRepository
{
    public async Task<WorkLog?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.WorkLogs
            .Include(w => w.User)
            .FirstOrDefaultAsync(w => w.Id == id, ct);
    }

    public async Task<IEnumerable<WorkLog>> GetByIssueIdAsync(int issueId, CancellationToken ct = default)
    {
        return await context.WorkLogs
            .Include(w => w.User)
            .Where(w => w.IssueId == issueId)
            .OrderByDescending(w => w.StartedAt)
            .ToListAsync(ct);
    }

    public async Task AddAsync(WorkLog workLog, CancellationToken ct = default)
    {
        await context.WorkLogs.AddAsync(workLog, ct);
    }

    public void Update(WorkLog workLog)
    {
        context.WorkLogs.Update(workLog);
    }

    public void Remove(WorkLog workLog)
    {
        context.WorkLogs.Remove(workLog);
    }
}