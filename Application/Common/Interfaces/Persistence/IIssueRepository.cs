namespace ProjectPlanner.Application.Common.Interfaces.Persistence;

public interface IIssueRepository
{
    Task<Issue?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Issue?> GetByKeyAndProjectIdAsync(string key, int projectId, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<int> GetCountByProjectIdAsync(int projectId, CancellationToken cancellationToken);
    Task<IEnumerable<Issue>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Issue>> FuzzySearchIssuesAsync(int projectId, string searchTerm, CancellationToken cancellationToken = default);
    Task AddAsync(Issue issue, CancellationToken cancellationToken = default);
    void Remove(Issue issue);
}