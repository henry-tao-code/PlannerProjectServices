using Contracts.Dtos;

namespace ProjectPlanner.Application.Common.Interfaces.AI;

public interface IHybridIndexIngestor
{
    Task IngestAsync(
        long attachmentId,
        IReadOnlyCollection<DocumentChunkDto> chunks,
        CancellationToken cancellationToken = default);
}
