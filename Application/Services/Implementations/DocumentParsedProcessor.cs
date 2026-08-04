using Contracts.Dtos;
using Contracts.Events;
using Domain.Entities;
using ProjectPlanner.Application.Common.Interfaces.AI;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Application.Services.Implementations;

public sealed class DocumentParsedProcessor(
    IDocumentChunkRepository chunkRepository,
    ITEIEmbeddingService embeddingService,
    IUnitOfWork unitOfWork)
    : IDocumentParsedProcessor
{

    public async Task ProcessAsync(
        DocumentParsedEvent parsedEvent,
        CancellationToken cancellationToken)
    {
        var chunks =
            parsedEvent.Chunks
            .Select(x => new DocumentChunk
            {
                AttachmentId = parsedEvent.AttachmentId,
                ChunkIndex = x.Index,
                Content = x.Content
            })
            .ToList();

        await chunkRepository.AddRangeAsync(
            chunks,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        var embeddingRequest =
            new EmbeddingRequestDto
            {
                Inputs = chunks
                    .Select(x => x.Content)
                    .ToList(),

                Truncate = true
            };

        var response =
            await embeddingService.GetEmbeddingsAsync(
                embeddingRequest,
                cancellationToken);


        if (response.Embeddings.Count != chunks.Count)
        {
            throw new Exception(
                "Embedding count mismatch");
        }

        for (int i = 0; i < chunks.Count; i++)
        {
            chunks[i].Embedding =
                response.Embeddings[i];
        }


        await unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}