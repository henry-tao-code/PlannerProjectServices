using Domain.Entities;
using ProjectPlanner.Application.Common.Dtos.AI;
using ProjectPlanner.Application.Common.Dtos.Search;
using ProjectPlanner.Application.Common.Interfaces.AI;

namespace ProjectPlanner.Application.Services.Implementations;

public class RetrievalService(
    IBm25SearchService bm25SearchService,
    IVectorSearchService vectorSearchService
    ) : IRetrievalService
{
    public async Task<IReadOnlyList<SearchResultDto>> SearchAsync(
        string query,
        QueryRoute route,
        int? projectId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return [];
        }

        var bm25Task = route.UseKeywordSearch
            ? bm25SearchService.SearchAsync(
                query,
                projectId,
                route.TopK,
                cancellationToken)
            : Task.FromResult<IReadOnlyList<SearchResultDto>>([]);

        var vectorTask = route.UseVectorSearch
            ? vectorSearchService.SearchAsync(
                query,
                projectId,
                route.TopK,
                cancellationToken)
            : Task.FromResult<IReadOnlyList<SearchResultDto>>([]);

        await Task.WhenAll(bm25Task, vectorTask);

        var bm25Results = await bm25Task;
        var vectorResults = await vectorTask;

        return CombineResults(
            bm25Results,
            vectorResults,
            route.TopK);
    }

    private static IReadOnlyList<SearchResultDto> CombineResults(
        IReadOnlyList<SearchResultDto> bm25Results,
        IReadOnlyList<SearchResultDto> vectorResults,
        int topK)
    {
        const double rrfK = 60.0;

        var combined = new Dictionary<string, CombinedResultDto>();

        AddResults(
            combined,
            bm25Results,
            rrfK);

        AddResults(
            combined,
            vectorResults,
            rrfK);

        return [.. combined.Values
            .OrderByDescending(x => x.Score)
             .Take(topK)
            .Select(x => new SearchResultDto(
                x.EntityId,
                x.EntityType,
                string.Empty,
                x.Title,
                x.Content,
                x.ProjectId,
                null,
                null,
                null,
                null,
                x.Score)
            {
                ChunkType = x.ChunkType,
                PageNumber = x.PageNumber,
                TableIndex = x.TableIndex
            })];
    }

    private static void AddResults(
        Dictionary<string, CombinedResultDto> combined,
        IReadOnlyList<SearchResultDto> results,
        double rrfK)
    {
        for (var index = 0; index < results.Count; index++)
        {
            var result = results[index];

            var key =
                $"{result.EntityType}:{result.EntityId}";

            if (!combined.TryGetValue(key, out var existing))
            {
                existing = new CombinedResultDto
                {
                    EntityId = result.EntityId,
                    EntityType = result.EntityType,
                    ProjectId = result.ProjectId,
                    Title = result.Title,
                    Content = result.Content ?? string.Empty,
                    ChunkType = result.ChunkType,
                    PageNumber = result.PageNumber,
                    TableIndex = result.TableIndex
                };

                combined[key] = existing;
            }

            var rank = index + 1;

            existing.Score +=
                1.0 / (rrfK + rank);
        }
    }
}
