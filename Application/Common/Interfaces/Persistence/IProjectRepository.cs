using Domain.Entities;

namespace ProjectPlanner.Application.Common.Interfaces.Persistence;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(Project project, CancellationToken ct = default);
    Task<IEnumerable<Project>> GetProjectsByUserIdAsync(int userId, CancellationToken ct = default);
    Task<bool> ExistsByKeyAsync(string key, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
}