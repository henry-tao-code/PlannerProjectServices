using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProjectPlanner.Application.Common.Dto.Issue;
using ProjectPlanner.Application.Services;

namespace ProjectPlanner.Api.Endpoints;

public static class IssueEndpoints
{
    public static IEndpointRouteBuilder MapIssueEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/issues")
            .WithTags("Issues");
        //.RequireAuthorization();
        group.MapPost("/", CreateIssueAsync);
        group.MapGet("/{id:int}", GetIssueByIdAsync);
        group.MapGet("/project/{projectId:int}", GetIssuesByProjectIdAsync);
        group.MapGet("/projects/{projectId:int}/issues/search", SearchProjectIssuesAsync);
        group.MapPut("/{id:int}", UpdateIssueAsync);
        group.MapPost("/{id:int}/move", MoveIssueAsync);
        group.MapDelete("/{id:int}", DeleteIssueAsync);
        return app;
    }

    private static async Task<IResult> CreateIssueAsync(
        IssueCreateDto dto,
        IIssueService issueService,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await issueService.CreateAsync(dto, cancellationToken);
            return Results.Created($"/api/issues/{result.Id}", result);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
    }

    private static async Task<IResult> GetIssueByIdAsync(
        int id,
        IIssueService issueService,
        CancellationToken cancellationToken)
    {
        var issue = await issueService.GetByIdAsync(id, cancellationToken);
        return issue is not null ? Results.Ok(issue) : Results.NotFound();
    }

    private static async Task<IResult> GetIssuesByProjectIdAsync(
    int projectId,
    IIssueService issueService,
    CancellationToken cancellationToken)
    {
        var issues = await issueService.GetByProjectAsync(projectId, cancellationToken);
        return Results.Ok(issues);
    }

    private static async Task<Results<Ok<IEnumerable<IssueSummaryDto>>, BadRequest<string>>> SearchProjectIssuesAsync(
        int projectId,
        [FromQuery] string? query,
        IIssueService issueService,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return TypedResults.BadRequest("Search query parameter cannot be empty.");
        }

        var results = await issueService.SearchIssuesAsync(projectId, query, ct);

        return TypedResults.Ok(results);
    }

    private static async Task<IResult> UpdateIssueAsync(
    int id,
    UpdateIssueCoreDto dto,
    IIssueService issueService,
    CancellationToken cancellationToken)
    {
        try
        {
            var updatedIssue = await issueService.UpdateCoreAsync(id, dto, cancellationToken);
            return Results.Ok(updatedIssue);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Results.Conflict(ex.Message);
        }
    }

    private static async Task<IResult> MoveIssueAsync(
    int id,
    MoveIssueDto dto,
    IIssueService issueService,
    CancellationToken cancellationToken)
    {
        try
        {
            var updatedIssue = await issueService.MoveIssueAsync(id, dto, cancellationToken);
            return Results.Ok(updatedIssue);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    private static async Task<IResult> DeleteIssueAsync(
        int id,
        IIssueService issueService,
        CancellationToken cancellationToken)
    {
        var deleted = await issueService.DeleteAsync(id, cancellationToken);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}