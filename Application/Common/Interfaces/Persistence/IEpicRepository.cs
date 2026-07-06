using Domain.Entities;

namespace ProjectPlanner.Application.Common.Interfaces.Persistence;

public interface IEpicRepository
{
    Task<Epic?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Epic?> GetByIdWithIssuesAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Epic>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
    Task AddAsync(Epic epic, CancellationToken cancellationToken = default);
    void Update(Epic epic);
    void Remove(Epic epic);
}