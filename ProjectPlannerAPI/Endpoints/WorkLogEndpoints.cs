using ProjectPlanner.Application.Common.Dto.WorkLog;
using ProjectPlanner.Application.Services;

namespace ProjectPlanner.Api.Endpoints;

public static class WorkLogEndpoints
{
    public static IEndpointRouteBuilder MapWorkLogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/worklogs").WithTags("WorkLogs");

        group.MapPost("/", CreateWorkLogAsync);
        group.MapGet("/issue/{issueId:int}", GetWorkLogsByIssueIdAsync);
        group.MapDelete("/{id:int}", DeleteWorkLogAsync);
        return app;
    }

    private static async Task<IResult> CreateWorkLogAsync(
        CreateWorkLogDto dto,
        IWorkLogService workLogService,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await workLogService.CreateAsync(dto, cancellationToken);
            return Results.Created($"/api/worklogs/issue/{result.IssueId}", result);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
    }

    private static async Task<IResult> GetWorkLogsByIssueIdAsync(
        int issueId,
        IWorkLogService workLogService,
        CancellationToken cancellationToken)
    {
        try
        {
            var logs = await workLogService.GetByIssueIdAsync(issueId, cancellationToken);
            return Results.Ok(logs);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
    }

    private static async Task<IResult> DeleteWorkLogAsync(
        int id,
        IWorkLogService workLogService,
        CancellationToken cancellationToken)
    {
        var deleted = await workLogService.DeleteAsync(id, cancellationToken);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}