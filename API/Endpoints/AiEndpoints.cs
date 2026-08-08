using ProjectPlanner.Application.Common.Dtos.AI;
using ProjectPlanner.Application.Services;

namespace ProjectPlanner.API.Endpoints;

public static class AiEndpoints
{
    public static IEndpointRouteBuilder MapAiEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/rag")
            .WithTags("RAG");

        group.MapPost("/query", async (
            RagQueryRequestDto request,
            IRagService ragService,
            CancellationToken cancellationToken) =>
        {
            var response = await ragService.QueryAsync(request, cancellationToken);
            return Results.Ok(response);
        })
        .WithName("QueryRag")
        .Produces<RagQueryResponseDto>(StatusCodes.Status200OK);

        //app.MapPost("/api/issues/{id:int}/generate-criteria", async (
        //    int id,
        //    IIssueRepository issueRepo,
        //    AiAssistantService aiService,
        //    CancellationToken ct) =>
        //{
        //    var issue = await issueRepo.GetByIdAsync(id, ct);
        //    if (issue is null) return Results.NotFound();

        //    var criteria = await aiService.GenerateAcceptanceCriteriaAsync(issue.Title, issue.Description ?? string.Empty, ct);

        //    return Results.Ok(new { Criteria = criteria });
        //});

        return app;

    }
}
