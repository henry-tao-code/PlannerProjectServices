using Domain.Entities;

namespace ProjectPlanner.Application.Common.Interfaces.Persistence;

public interface IDocumentChunkRepository
{
    Task AddAsync(
        DocumentChunk chunk,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(
        IEnumerable<DocumentChunk> chunks,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DocumentChunk>> GetByAttachmentIdAsync(
        long attachmentId,
        CancellationToken cancellationToken = default);

    Task DeleteByAttachmentIdAsync(
        long attachmentId,
        CancellationToken cancellationToken = default);
}