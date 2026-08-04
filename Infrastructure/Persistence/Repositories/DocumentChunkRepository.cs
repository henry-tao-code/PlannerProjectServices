using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Infrastructure.Persistence.Repositories;

public sealed class DocumentChunkRepository(
    ProjectPlannerDbContext dbContext)
    : IDocumentChunkRepository
{

    public async Task AddAsync(
        DocumentChunk chunk,
        CancellationToken cancellationToken = default)
    {
        await dbContext.DocumentChunks
            .AddAsync(
                chunk,
                cancellationToken);
    }

    public async Task AddRangeAsync(
        IEnumerable<DocumentChunk> chunks,
        CancellationToken cancellationToken = default)
    {
        await dbContext.DocumentChunks
            .AddRangeAsync(
                chunks,
                cancellationToken);
    }

    public async Task<IReadOnlyList<DocumentChunk>>
        GetByAttachmentIdAsync(
            long attachmentId,
            CancellationToken cancellationToken = default)
    {
        return await dbContext.DocumentChunks
            .Where(x => x.AttachmentId == attachmentId)
            .OrderBy(x => x.ChunkIndex)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteByAttachmentIdAsync(
        long attachmentId,
        CancellationToken cancellationToken = default)
    {
        var chunks =
            await dbContext.DocumentChunks
                .Where(x => x.AttachmentId == attachmentId)
                .ToListAsync(cancellationToken);

        dbContext.DocumentChunks.RemoveRange(chunks);
    }
}
