using ProjectPlanner.Application.Common.Dtos.AI;
using ProjectPlanner.Application.Common.Interfaces.AI;

namespace ProjectPlanner.Application.Services.Implementations;

public class RagService(
    IQueryRouterService queryRouterService,
    IRetrievalService retrievalService,
    IPromptBuilderService promptBuilderService,
    ILlmService llmService
    //, ILogger<RagService> logger
    ) : IRagService
{
    public async Task<RagQueryResponseDto> QueryAsync(
        RagQueryRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Query))
        {
            throw new ArgumentException(
                "A RAG query is required.",
                nameof(request));
        }

        var route = queryRouterService.Route(request.Query);

        var results = await retrievalService.SearchAsync(
            request.Query,
            route,
            request.ProjectId,
            cancellationToken);

        //if (results.Count == 0)
        //{
        //    logger.LogWarning(
        //        "RAG retrieval returned 0 results in {ElapsedMs}ms for Query: '{Query}', Intent: {Intent}, ProjectId: {ProjectId}",
        //        stopwatch.ElapsedMilliseconds,
        //        request.Query,
        //        route.Intent,
        //        request.ProjectId);
        //}
        //else
        //{
        //    logger.LogInformation(
        //        "RAG retrieval found {Count} results in {ElapsedMs}ms (Top Score: {TopScore})",
        //        results.Count,
        //        stopwatch.ElapsedMilliseconds,
        //       results.Max(r => r.Score));
        //} 

        var sources = results
            .Select(result => new RagSourceDto
            {
                ChunkId = result.EntityId,
                DocumentName = result.Title,
                Content = result.Content ?? string.Empty,
                Score = result.Score ?? 0,
                ChunkType = result.ChunkType,
                PageNumber = result.PageNumber,
                TableIndex = result.TableIndex
            })
            .ToList();

        if (sources.Count == 0)
        {
            return new RagQueryResponseDto
            {
                Answer = "I couldn't find relevant information in the indexed project documents.",
                Intent = route.Intent,
                Sources = sources
            };
        }

        var prompt = promptBuilderService.Build(
            request.Query,
            sources,
            route);

        var answer = await llmService.GenerateAsync(
            prompt,
            cancellationToken);

        return new RagQueryResponseDto
        {
            Answer = answer,
            Intent = route.Intent,
            Sources = sources
        };
    }
}
