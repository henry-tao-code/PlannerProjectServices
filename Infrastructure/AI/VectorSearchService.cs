using Contracts.Dtos;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using ProjectPlanner.Application.Common.Dtos.Search;
using ProjectPlanner.Application.Common.Interfaces.AI;
using ProjectPlanner.Infrastructure.Persistence;

namespace ProjectPlanner.Infrastructure.AI;

public class VectorSearchService(
    ProjectPlannerDbContext dbContext,
    ITEIEmbeddingService embeddingService
    // ,ILogger<VectorSearchService> logger
    ) : IVectorSearchService
{
    public async Task<IReadOnlyList<SearchResultDto>> SearchAsync(
        string query,
        int? projectId,
        int topK,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return [];
        }

        var request = new EmbeddingRequestDto
        {
            Inputs = [query.Trim()],
            Truncate = true
        };

        // var stopwatch = Stopwatch.StartNew();

        var embeddingResponse = await embeddingService.GetEmbeddingsAsync(
            request,
            cancellationToken);

        if (embeddingResponse.Embeddings.Count != 1)
        {
            throw new InvalidOperationException(
                "The embedding service did not return an embedding for the query.");
        }

        Vector queryVector = new(embeddingResponse.Embeddings[0]);

        var queryable = dbContext.DocumentChunks
            .AsNoTracking()
            .Where(x => x.Embedding != null);

        if (projectId.HasValue)
        {
            queryable = queryable.Where(
                x => x.Attachment.Issue.ProjectId == projectId.Value);
        }

        var results = await queryable
            .Select(x => new
            {
                EntityId = x.Id,
                EntityType = "Document",
                x.Attachment.Issue.ProjectId,
                IssueId = (int?)x.Attachment.IssueId,
                Title = x.Attachment.OriginalFileName,
                x.Content,
                x.ChunkType,
                x.PageNumber,
                x.TableIndex,

                Distance = x.Embedding!.CosineDistance(queryVector)
            })
            .OrderBy(x => x.Distance)
            .Take(topK)
            .ToListAsync(cancellationToken);

        // stopwatch.Stop();

        // logger.LogWarning(
        //         "RAG retrieval through vector search in {ElapsedMs}ms",
        //         stopwatch.ElapsedMilliseconds);

        return [.. results
            .Select(x => new SearchResultDto(
                x.EntityId, x.EntityType, string.Empty, x.Title, x.Content,
                x.ProjectId, null, null, null, null, 1.0 - x.Distance)
            {
                ChunkType = x.ChunkType,
                PageNumber = x.PageNumber,
                TableIndex = x.TableIndex
            })];
    }
}
