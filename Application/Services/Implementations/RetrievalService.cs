using Domain.Entities;
using ProjectPlanner.Application.Common.Dtos.AI;
using ProjectPlanner.Application.Common.Dtos.Search;
using ProjectPlanner.Application.Common.Interfaces.AI;

namespace ProjectPlanner.Application.Services.Implementations;

public class RetrievalService(
    IKeywordSearchService keywordSearchService,
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

        var keywordTask = route.UseKeywordSearch
            ? keywordSearchService.SearchAsync(
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

        await Task.WhenAll(keywordTask, vectorTask);

        var keywordResults = await keywordTask;
        var vectorResults = await vectorTask;

        return CombineResults(
            keywordResults,
            vectorResults,
            route.TopK);
    }

    private static IReadOnlyList<SearchResultDto> CombineResults(
        IReadOnlyList<SearchResultDto> keywordResults,
        IReadOnlyList<SearchResultDto> vectorResults,
        int topK)
    {
        const double rrfK = 60.0;

        var combined = new Dictionary<string, CombinedResultDto>();

        AddResults(
            combined,
            keywordResults,
            rrfK);

        AddResults(
            combined,
            vectorResults,
            rrfK);

        return [.. combined.Values
            .OrderByDescending(x => x.Score)
            .Take(topK)
            .Select(x => new SearchResultDto
            {
                EntityId = x.EntityId,
                EntityType = x.EntityType,
                ProjectId = x.ProjectId,
                Title = x.Title,
                Content = x.Content,
                Score = x.Score
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
                    Content = result.Content
                };

                combined[key] = existing;
            }

            var rank = index + 1;

            existing.Score +=
                1.0 / (rrfK + rank);
        }
    }
}
