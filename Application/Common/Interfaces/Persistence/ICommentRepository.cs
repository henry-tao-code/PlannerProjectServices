using Domain.Entities;

namespace ProjectPlanner.Application.Common.Interfaces.Persistence;

public interface ICommentRepository
{
    Task<IssueComment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<IssueComment>> GetByIssueIdAsync(int issueId, CancellationToken cancellationToken = default);
    Task AddAsync(IssueComment comment, CancellationToken cancellationToken = default);
    void Update(IssueComment comment);
    void Remove(IssueComment comment);
}