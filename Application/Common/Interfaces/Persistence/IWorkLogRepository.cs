using Domain.Entities;

namespace ProjectPlanner.Application.Common.Interfaces.Persistence;

public interface IWorkLogRepository
{
    Task<WorkLog?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkLog>> GetByIssueIdAsync(int issueId, CancellationToken cancellationToken = default);
    Task AddAsync(WorkLog workLog, CancellationToken cancellationToken = default);
}