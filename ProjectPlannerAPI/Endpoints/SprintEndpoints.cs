using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProjectPlanner.Application.Common.Dto.Sprint;
using ProjectPlanner.Application.Services;

namespace ProjectPlanner.Api.Endpoints;

public static class SprintEndpoints
{
    public static IEndpointRouteBuilder MapSprintEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sprints")
            .WithTags("Sprints");

        group.MapPost("/", CreateSprintAsync);
        group.MapGet("/{id:int}", GetSprintByIdAsync);
        group.MapGet("/project/{projectId:int}", GetSprintsByProjectIdAsync);
        group.MapPut("/{id:int}", UpdateSprintAsync);
        group.MapPost("/{id:int}/start", StartSprintAsync);
        group.MapPost("/{id:int}/complete", CompleteSprintAsync);

        group.MapDelete("/{id:int}", DeleteSprintAsync);

        return group;
    }

    // ---------------- CREATE ----------------
    private static async Task<Results<Created<SprintDto>, BadRequest<string>>> CreateSprintAsync(
        [FromBody] CreateSprintDto dto,
        ISprintService sprintService,
        CancellationToken ct)
    {
        try
        {
            var result = await sprintService.CreateAsync(dto, ct);
            return TypedResults.Created($"/api/sprints/{result.Id}", result);
        }
        catch (KeyNotFoundException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    // ---------------- GET BY ID ----------------
    private static async Task<Results<Ok<SprintDto>, NotFound>> GetSprintByIdAsync(
        int id,
        ISprintService sprintService,
        CancellationToken ct)
    {
        var sprint = await sprintService.GetByIdAsync(id, ct);
        return sprint is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(sprint);
    }

    // ---------------- GET BY PROJECT ----------------
    private static async Task<Results<Ok<IEnumerable<SprintDto>>, NotFound>> GetSprintsByProjectIdAsync(
        int projectId,
        ISprintService sprintService,
        CancellationToken ct)
    {
        var sprints = await sprintService.GetByProjectIdAsync(projectId, ct);
        return TypedResults.Ok(sprints);
    }

    // ---------------- UPDATE ----------------
    private static async Task<Results<
        Ok<SprintDto>,
        NotFound<string>,
        Conflict<string>,
        BadRequest<string>>>
    UpdateSprintAsync(
        int id,
        [FromBody] UpdateSprintDto dto,
        ISprintService sprintService,
        CancellationToken ct)
    {
        try
        {
            var updated = await sprintService.UpdateAsync(id, dto, ct);
            return TypedResults.Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return TypedResults.NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Conflict(ex.Message);
        }
    }

    // ---------------- START ----------------
    private static async Task<Results<Ok<SprintDto>, NotFound>> StartSprintAsync(
        int id,
        ISprintService sprintService,
        CancellationToken ct)
    {
        try
        {
            var updated = await sprintService.StartSprintAsync(id, ct);
            return TypedResults.Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return TypedResults.NotFound();
        }
    }

    // ---------------- COMPLETE ----------------
    private static async Task<Results<
        Ok<SprintDto>,
        NotFound<string>,
        BadRequest<string>>>
    CompleteSprintAsync(
        int id,
        [FromQuery] int? targetSprintIdForRollover,
        ISprintService sprintService,
        CancellationToken ct)
    {
        try
        {
            var updated = await sprintService.CompleteSprintAsync(
                id,
                targetSprintIdForRollover,
                ct);

            return TypedResults.Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return TypedResults.NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    // ---------------- DELETE ----------------
    private static async Task<Results<NoContent, NotFound>> DeleteSprintAsync(
        int id,
        ISprintService sprintService,
        CancellationToken ct)
    {
        var deleted = await sprintService.DeleteAsync(id, ct);

        return deleted
            ? TypedResults.NoContent()
            : TypedResults.NotFound();
    }
}