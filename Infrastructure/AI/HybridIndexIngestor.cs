using Contracts.Dtos;
using Domain.Entities;
using Pgvector;
using ProjectPlanner.Application.Common.Interfaces.AI;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Infrastructure.AI;

public class HybridIndexIngestor(
    IDocumentChunkRepository chunkRepository,
    ITEIEmbeddingService embeddingService,
    IUnitOfWork unitOfWork) : IHybridIndexIngestor
{
    public async Task IngestAsync(
        long attachmentId,
        IReadOnlyCollection<DocumentChunkDto> sourceChunks,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(attachmentId);
        ArgumentNullException.ThrowIfNull(sourceChunks);

        var chunks = sourceChunks
            .OrderBy(chunk => chunk.Index)
            .Select(chunk =>
            {
                if (string.IsNullOrWhiteSpace(chunk.Content))
                {
                    throw new ArgumentException(
                        $"Chunk {chunk.Index} cannot be empty.",
                        nameof(sourceChunks));
                }

                return new DocumentChunk
                {
                    AttachmentId = attachmentId,
                    ChunkIndex = chunk.Index,
                    Content = chunk.Content.Trim(),
                    TokenCount = chunk.TokenCount,
                    ChunkType = string.IsNullOrWhiteSpace(chunk.Type) ? "content" : chunk.Type.Trim(),
                    PageNumber = chunk.Page,
                    TableIndex = chunk.TableIndex
                };
            })
            .ToList();

        if (chunks.Count == 0)
        {
            await unitOfWork.ExecuteInTransactionAsync(async ct =>
            {
                await chunkRepository.DeleteByAttachmentIdAsync(attachmentId, ct);
                await unitOfWork.SaveChangesAsync(ct);
            }, cancellationToken);
            return;
        }

        var embeddingResponse = await embeddingService.GetEmbeddingsAsync(
            new EmbeddingRequestDto
            {
                Inputs = [.. chunks.Select(chunk => chunk.Content)],
                Truncate = true
            },
            cancellationToken);

        if (embeddingResponse.Embeddings.Count != chunks.Count)
        {
            throw new InvalidOperationException(
                "The embedding service returned a different number of vectors than input chunks.");
        }

        for (var index = 0; index < chunks.Count; index++)
        {
            chunks[index].Embedding = new Vector(embeddingResponse.Embeddings[index]);
        }

        await unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            await chunkRepository.DeleteByAttachmentIdAsync(attachmentId, ct);
            await chunkRepository.AddRangeAsync(chunks, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }, cancellationToken);
    }
}
