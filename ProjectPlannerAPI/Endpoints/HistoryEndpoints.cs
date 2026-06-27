using Microsoft.AspNetCore.Http.HttpResults;
using ProjectPlanner.Application.Common.Dto.History;

namespace ProjectPlanner.Api.Endpoints;

public static class HistoryEndpoints
{
    public static IEndpointRouteBuilder MapHistoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/history")
            .WithTags("History");

        group.MapGet("/issue/{issueId:int}", GetHistoryByIssueIdAsync);

        return app;
    }

    private static async Task<Results<Ok<IEnumerable<HistoryDto>>, NotFound<string>>> GetHistoryByIssueIdAsync(
        int issueId,
        IHistoryService historyService,
        CancellationToken cancellationToken)
    {
        try
        {
            var logs = await historyService.GetByIssueIdAsync(issueId, cancellationToken);
            return TypedResults.Ok(logs);
        }
        catch (KeyNotFoundException ex)
        {
            return TypedResults.NotFound(ex.Message);
        }
    }
}