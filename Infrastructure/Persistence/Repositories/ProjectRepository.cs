using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ProjectPlanner.Application.Common.Interfaces.Persistence;
using ProjectPlanner.Infrastructure.Persistence;

namespace ProjectPlanner.Infrastructure.Persistence.Repositories;

public class ProjectRepository(ProjectPlannerDbContext dbContext) : IProjectRepository
{
    private readonly ProjectPlannerDbContext _dbContext = dbContext;

    public async Task<Project?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _dbContext.Projects
            .Include(p => p.Lead)
            .Include(p => p.ProjectMembers)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task AddAsync(Project project, CancellationToken ct)
    {
        await _dbContext.Projects.AddAsync(project, ct);
    }

    public async Task<IEnumerable<Project>> GetProjectsByUserIdAsync(int userId, CancellationToken ct = default)
    {
        return await _dbContext.Projects
            .AsNoTracking()
            .Include(p => p.Lead)
            .Include(p => p.ProjectMembers)
                .ThenInclude(m => m.User)
            .Where(p =>
                !p.IsArchived &&
                (p.LeadId == userId || p.ProjectMembers.Any(m => m.UserId == userId && !m.IsDeleted)))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsByKeyAsync(string key, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            return false;

        key = key.Trim().ToUpperInvariant();

        return await _dbContext.Projects
            .AnyAsync(p => p.Key == key && !p.IsArchived, ct);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return await _dbContext.Projects
            .AnyAsync(p => p.Id == id && !p.IsArchived, ct);
    }
}