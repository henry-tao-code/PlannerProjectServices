using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProjectPlanner.Application.Common.Dtos.Epic;
using ProjectPlanner.Application.Services;

namespace ProjectPlanner.Api.Endpoints;

public static class EpicEndpoints
{
    public static void MapEpicEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/epic")
            .WithTags("Epic");

        group.MapPost("/", CreateEpicAsync);
        group.MapGet("/{id:int}", GetEpicByIdAsync);
        group.MapGet("/{id:int}/issues", GetEpicWithIssuesAsync);
        group.MapGet("/project/{projectId:int}", GetEpicsByProjectAsync);
        group.MapPut("/{id:int}", UpdateEpicAsync);
        group.MapDelete("/{id:int}", DeleteEpicAsync);
    }

    private static async Task<Results<Created<EpicDto>, NotFound, BadRequest<string>>> CreateEpicAsync(
        [FromBody] CreateEpicDto dto,
        IEpicService epicService,
        CancellationToken ct)
    {
        try
        {
            var result = await epicService.CreateAsync(dto, ct);
            return TypedResults.Created($"/api/epics/{result.Id}", result);
        }
        catch (KeyNotFoundException)
        {
            return TypedResults.NotFound();
        }
        catch (ArgumentException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    private static async Task<Results<Ok<EpicDto>, NotFound>> GetEpicByIdAsync(
        int id,
        IEpicService epicService,
        CancellationToken ct)
    {
        var result = await epicService.GetByIdAsync(id, ct);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<EpicDto>, NotFound>> GetEpicWithIssuesAsync(
        int id,
        IEpicService epicService,
        CancellationToken ct)
    {
        var result = await epicService.GetByIdWithIssuesAsync(id, ct);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<IEnumerable<EpicSummaryDto>>, NotFound>> GetEpicsByProjectAsync(
        int projectId,
        IEpicService epicService,
        CancellationToken ct)
    {
        try
        {
            var result = await epicService.GetByProjectAsync(projectId, ct);
            return TypedResults.Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return TypedResults.NotFound();
        }
    }

    private static async Task<Results<Ok<EpicDto>, NotFound, BadRequest<string>>> UpdateEpicAsync(
        int id,
        [FromBody] UpdateEpicDto dto,
        IEpicService epicService,
        CancellationToken ct)
    {
        try
        {
            var result = await epicService.UpdateAsync(id, dto, ct);
            return TypedResults.Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return TypedResults.NotFound();
        }
        catch (InvalidOperationException ex) // Catch concurrency conflicts or date rule breaks
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (ArgumentException ex) // Catch validation rule breaks
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    private static async Task<Results<NoContent, NotFound>> DeleteEpicAsync(
        int id,
        IEpicService epicService,
        CancellationToken ct)
    {
        var succeeded = await epicService.DeleteAsync(id, ct);
        return succeeded ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}