using ProjectPlanner.Application.Common.Dtos.ProjectMember;
using ProjectPlanner.Application.Services;
using System.Security.Claims;

namespace ProjectPlanner.Api.Endpoints;

public static class ProjectMemberEndpoints
{
    public static IEndpointRouteBuilder MapProjectMemberEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/projects/{projectId:int}/members")
            .WithTags("Project Members");

        group.MapGet("/", GetMembersAsync);
        group.MapPost("/", AddMemberAsync);
        group.MapPut("/{userId:int}", ChangeRoleAsync);
        group.MapDelete("/{userId:int}", RemoveMemberAsync);

        return group;
    }

    private static int GetUserId(ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? user.FindFirst("sub")?.Value;

        if (!int.TryParse(userId, out var id))
            throw new UnauthorizedAccessException();

        return id;
    }

    // ---------------- GET MEMBERS ----------------
    private static async Task<IResult> GetMembersAsync(
        int projectId,
        IProjectMemberService service,
        CancellationToken ct)
    {
        var members = await service.GetMembersAsync(projectId, ct);
        return Results.Ok(members);
    }

    // ---------------- ADD MEMBER ----------------
    private static async Task<IResult> AddMemberAsync(
        int projectId,
        AddProjectMemberDto dto,
        ClaimsPrincipal user,
        IProjectMemberService service,
        CancellationToken ct)
    {
        try
        {
            var userId = GetUserId(user);

            var result = await service.AddMemberAsync(
                projectId,
                dto.UserId,
                dto.Role,
                userId,
                ct);

            return Results.Created(
                $"/api/projects/{projectId}/members/{dto.UserId}",
                result);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(ex.Message);
        }
        catch
        {
            return Results.Unauthorized();
        }
    }

    // ---------------- CHANGE ROLE ----------------
    private static async Task<IResult> ChangeRoleAsync(
        int projectId,
        int userId,
        UpdateProjectMemberRoleDto dto,
        ClaimsPrincipal user,
        IProjectMemberService service,
        CancellationToken ct)
    {
        try
        {
            var currentUserId = GetUserId(user);

            await service.ChangeRoleAsync(
                projectId,
                userId,
                dto.Role,
                currentUserId,
                ct);

            return Results.Ok();
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

    // ---------------- REMOVE MEMBER ----------------
    private static async Task<IResult> RemoveMemberAsync(
        int projectId,
        int userId,
        IProjectMemberService service,
        CancellationToken ct)
    {
        try
        {
            await service.RemoveMemberAsync(projectId, userId, ct);
            return Results.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
    }
}