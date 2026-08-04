using Domain.Entities;

namespace ProjectPlanner.Application.Common.Interfaces.Persistence;

public interface IIssueAttachmentRepository
{
    Task<IssueAttachment?> GetByIdAsync(
        long id,
        CancellationToken ct = default);

    Task<List<IssueAttachment>> GetByIssueIdAsync(
        int issueId,
        CancellationToken ct = default);

    Task AddAsync(
        IssueAttachment attachment,
        CancellationToken ct = default);

    void Update(IssueAttachment attachment);

    void Remove(IssueAttachment attachment);
}