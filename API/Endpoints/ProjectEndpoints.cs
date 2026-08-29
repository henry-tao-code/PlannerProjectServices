using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProjectPlanner.Application.Common;
using ProjectPlanner.Application.Common.Dtos.Project;
using ProjectPlanner.Application.Common.Dtos.Search;
using ProjectPlanner.Application.Services;
using System.Security.Claims;

namespace ProjectPlanner.API.Endpoints;

public static class ProjectEndpoints
{
    public static IEndpointRouteBuilder MapProjectEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/projects")
            .WithTags("Projects");

        group.MapGet("/", ListProjectsAsync);
        group.MapPost("/", CreateProjectAsync);

        group.MapGet("/{id:int}/summary", GetProjectSummaryAsync);
        group.MapGet("/{id:int}/timeline", GetProjectTimelineAsync);
        group.MapGet("/{id:int}/backlog", GetProjectBacklogAsync);
        group.MapGet("/{id:int}/board", GetProjectBoardAsync);
        group.MapGet("/{id:int}/calendar", GetProjectCalendarAsync);
        group.MapGet("/{id:int}/list", GetProjectListAsync);
        group.MapGet("/{id:int}/development", GetProjectDevelopmentDetailsAsync);
        group.MapGet("/{id:int}/archived", GetArchivedWorkAsync);
        group.MapGet("/{id:int}/settings", GetProjectSettingsAsync);
        group.MapPut("/{id:int}/settings", UpdateProjectSettingsAsync);

        // group.MapGet("/{projectId:int}/search", SearchProjectAsync);
        app.MapGet("/api/projects/{projectId}/search", SearchProjectAsync).AllowAnonymous();

        return app;
    }

    private static int GetUserIdOrThrow(ClaimsPrincipal user)
    {
        var userIdClaim =
            user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
            user.FindFirst("sub")?.Value;

        if (!int.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException();

        return userId;
    }

    private static UnauthorizedHttpResult Unauthorized() => TypedResults.Unauthorized();

    // ---------------- PROJECT LIST ----------------

    private static async Task<Results<Ok<IEnumerable<ProjectDashboardResponseDto>>, UnauthorizedHttpResult>>
    ListProjectsAsync(
        ClaimsPrincipal userPrincipal,
        IProjectService projectService,
        CancellationToken ct)
    {
        try
        {
            var userId = GetUserIdOrThrow(userPrincipal);
            var projects = await projectService.GetUserProjectsAsync(userId, ct);
            return TypedResults.Ok(projects);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    // ---------------- CREATE ----------------

    private static async Task<Results<Created<ProjectDashboardResponseDto>, UnauthorizedHttpResult, BadRequest<string>>>
    CreateProjectAsync(
        ClaimsPrincipal userPrincipal,
        [FromBody] CreateProjectDto dto,
        IProjectService projectService,
        CancellationToken ct)
    {
        try
        {
            var userId = GetUserIdOrThrow(userPrincipal);
            var newProject = await projectService.CreateProjectAsync(userId, dto, ct);
            return TypedResults.Created($"/api/projects/{newProject.Id}", newProject);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    // ---------------- READ (COMMON PATTERN) ----------------

    private static async Task<Results<Ok<ProjectSummaryDto>, NotFound, UnauthorizedHttpResult>>
    GetProjectSummaryAsync(int id, ClaimsPrincipal userPrincipal, IProjectService service, CancellationToken ct)
    {
        try
        {
            GetUserIdOrThrow(userPrincipal);

            var result = await service.GetProjectSummaryAsync(id, ct);
            return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    private static async Task<Results<Ok<ProjectTimelineDto>, NotFound, UnauthorizedHttpResult>>
    GetProjectTimelineAsync(int id, ClaimsPrincipal userPrincipal, IProjectService service, CancellationToken ct)
    {
        try
        {
            GetUserIdOrThrow(userPrincipal);

            var result = await service.GetProjectTimelineAsync(id, ct);
            return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    private static async Task<Results<Ok<ProjectBacklogDto>, NotFound, UnauthorizedHttpResult>>
    GetProjectBacklogAsync(int id, ClaimsPrincipal userPrincipal, IProjectService service, CancellationToken ct)
    {
        try
        {
            GetUserIdOrThrow(userPrincipal);

            var result = await service.GetProjectBacklogAsync(id, ct);
            return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    private static async Task<Results<Ok<ProjectBoardDto>, NotFound, UnauthorizedHttpResult>>
    GetProjectBoardAsync(int id, ClaimsPrincipal userPrincipal, IProjectService service, CancellationToken ct)
    {
        try
        {
            GetUserIdOrThrow(userPrincipal);

            var result = await service.GetProjectBoardAsync(id, ct);
            return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    private static async Task<Results<Ok<IEnumerable<ProjectCalendarEventDto>>, UnauthorizedHttpResult>>
    GetProjectCalendarAsync(int id, ClaimsPrincipal userPrincipal, IProjectService service, CancellationToken ct)
    {
        try
        {
            GetUserIdOrThrow(userPrincipal);
            var result = await service.GetProjectCalendarAsync(id, ct);
            return TypedResults.Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    private static async Task<Results<Ok<IEnumerable<ProjectIssueListDto>>, UnauthorizedHttpResult>>
    GetProjectListAsync(int id, ClaimsPrincipal userPrincipal, IProjectService service, CancellationToken ct)
    {
        try
        {
            GetUserIdOrThrow(userPrincipal);
            var result = await service.GetProjectListAsync(id, ct);
            return TypedResults.Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    private static async Task<Results<Ok<ProjectDevelopmentDto>, NotFound, UnauthorizedHttpResult>>
    GetProjectDevelopmentDetailsAsync(int id, ClaimsPrincipal userPrincipal, IProjectService service, CancellationToken ct)
    {
        try
        {
            GetUserIdOrThrow(userPrincipal);

            var result = await service.GetProjectDevelopmentDetailsAsync(id, ct);
            return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    private static async Task<Results<Ok<ArchivedWorkDto>, UnauthorizedHttpResult>>
    GetArchivedWorkAsync(int id, ClaimsPrincipal userPrincipal, IProjectService service, CancellationToken ct)
    {
        try
        {
            GetUserIdOrThrow(userPrincipal);
            var result = await service.GetArchivedWorkAsync(id, ct);
            return TypedResults.Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    // ---------------- SETTINGS ----------------

    private static async Task<Results<Ok<ProjectSettingsDto>, NotFound, UnauthorizedHttpResult>>
    GetProjectSettingsAsync(int id, ClaimsPrincipal userPrincipal, IProjectService service, CancellationToken ct)
    {
        try
        {
            GetUserIdOrThrow(userPrincipal);

            var result = await service.GetProjectSettingsAsync(id, ct);
            return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    private static async Task<Results<Ok<ProjectSettingsDto>, NotFound, Conflict<string>, UnauthorizedHttpResult, BadRequest<string>>>
    UpdateProjectSettingsAsync(
        int id,
        [FromHeader(Name = "X-Row-Version")] uint clientRowVersion,
        [FromBody] UpdateProjectSettingsDto dto,
        ClaimsPrincipal userPrincipal,
        IProjectService service,
        CancellationToken ct)
    {
        try
        {
            GetUserIdOrThrow(userPrincipal);

            var result = await service.UpdateProjectSettingsAsync(id, dto, clientRowVersion, ct);
            return TypedResults.Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (KeyNotFoundException)
        {
            return TypedResults.NotFound();
        }
        catch (ConcurrencyException ex)
        {
            return TypedResults.Conflict(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    // ---------------- SEARCH ----------------

    //private static async Task<Results<Ok<IEnumerable<SearchResultDto>>, BadRequest<string>, UnauthorizedHttpResult>>
    //SearchProjectAsync(
    //    int projectId,
    //    [FromQuery] string? query,
    //    ClaimsPrincipal userPrincipal,
    //    ISearchService searchService,
    //    CancellationToken ct)
    //{
    //    try
    //    {
    //        GetUserIdOrThrow(userPrincipal);

    //        if (string.IsNullOrWhiteSpace(query))
    //        {
    //            return TypedResults.BadRequest("Search query parameter cannot be empty.");
    //        }

    //        var results = await searchService.SearchAsync(
    //            new SearchRequestDto(query.Trim(), projectId, PageSize: 50),
    //            ct);

    //        return TypedResults.Ok<IEnumerable<SearchResultDto>>(results.Items);
    //    }
    //    catch (UnauthorizedAccessException)
    //    {
    //        return Unauthorized();
    //    }
    //}

    private static async Task<Results<Ok<IEnumerable<SearchResultDto>>, BadRequest<string>>> SearchProjectAsync(
    int projectId,
    [FromQuery] string? query,
    ISearchService searchService,
    CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return TypedResults.BadRequest("Search query parameter cannot be empty.");
        }

        var results = await searchService.SearchAsync(
            new SearchRequestDto(query.Trim(), projectId, PageSize: 50),
            ct);

        return TypedResults.Ok<IEnumerable<SearchResultDto>>(results.Items);
    }
}
