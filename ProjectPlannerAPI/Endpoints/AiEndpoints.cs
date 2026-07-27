using ProjectPlanner.Application.Common.Interfaces.Persistence;
using ProjectPlanner.Application.Services.Implementations;

namespace ProjectPlanner.API.Endpoints;

public static class AiEndpoints
{
    public static IEndpointRouteBuilder MapAiEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/issues/{id:int}/generate-criteria", async (
            int id,
            IIssueRepository issueRepo,
            AiAssistantService aiService,
            CancellationToken ct) =>
        {
            var issue = await issueRepo.GetByIdAsync(id, ct);
            if (issue is null) return Results.NotFound();

            var criteria = await aiService.GenerateAcceptanceCriteriaAsync(issue.Title, issue.Description, ct);

            return Results.Ok(new { Criteria = criteria });
        });
        return app;
    }
}
