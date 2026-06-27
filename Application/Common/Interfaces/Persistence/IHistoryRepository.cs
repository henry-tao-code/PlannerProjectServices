using Domain.Entities;

namespace ProjectPlanner.Application.Common.Interfaces.Persistence;

public interface IHistoryRepository
{
    Task<IssueHistory?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<IssueHistory>> GetByIssueIdAsync(int issueId, CancellationToken cancellationToken = default);
    Task AddAsync(IssueHistory history, CancellationToken cancellationToken = default);
}