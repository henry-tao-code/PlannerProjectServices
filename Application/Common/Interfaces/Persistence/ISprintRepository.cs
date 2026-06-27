using Domain.Entities;

namespace ProjectPlanner.Application.Common.Interfaces.Persistence;

public interface ISprintRepository
{
    Task<Sprint?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sprint>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
    Task<Sprint?> GetActiveSprintByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
    Task<Sprint?> GetBacklogSprintByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
    Task AddAsync(Sprint sprint, CancellationToken cancellationToken = default);
    void Delete(Sprint sprint);
}