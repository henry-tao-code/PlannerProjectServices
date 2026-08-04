using Microsoft.AspNetCore.Http.HttpResults;
using ProjectPlanner.API.Filters;
using ProjectPlanner.Application.Common;
using ProjectPlanner.Application.Common.Dtos.Auth;
using ProjectPlanner.Application.Common.Dtos.User;
using ProjectPlanner.Application.Services;
using System.Security.Claims;

namespace ProjectPlanner.API.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users");

        group.MapPost("/register", RegisterUserAsync)
            .AddEndpointFilter<ValidationFilter<CreateUserDto>>();

        group.MapPost("/login", LoginUserAsync)
            .AddEndpointFilter<ValidationFilter<LoginRequestDto>>();

        group.MapPost("/refresh", RefreshTokenAsync)
            .AddEndpointFilter<ValidationFilter<RefreshTokenRequestDto>>();

        group.MapGet("/me", GetCurrentUserAsync)
            .RequireAuthorization();

        group.MapGet("/username/{username}", GetByUsernameAsync);

        group.MapPut("/{id:int}", UpdateProfileAsync)
            .RequireAuthorization();

        group.MapDelete("/{id:int}", DeleteAccountAsync)
            .RequireAuthorization();

        return app;
    }

    // ---------------- REGISTER ----------------

    private static async Task<Results<Created<UserResponseDto>, BadRequest<string>>> RegisterUserAsync(
        CreateUserDto dto,
        IUserService userService,
        CancellationToken ct)
    {
        try
        {
            var result = await userService.RegisterAsync(dto, ct);
            return TypedResults.Created($"/api/users/{result.Id}", result);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    // ---------------- LOGIN ----------------

    private static async Task<Results<Ok<AuthResponseDto>, UnauthorizedHttpResult, BadRequest<string>>> LoginUserAsync(
        LoginRequestDto dto,
        IUserService userService,
        CancellationToken ct)
    {
        try
        {
            var result = await userService.LoginAsync(dto, ct);
            return TypedResults.Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Unauthorized();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    // ---------------- REFRESH ----------------

    private static async Task<Results<Ok<AuthResponseDto>, UnauthorizedHttpResult>> RefreshTokenAsync(
        RefreshTokenRequestDto dto,
        IUserService userService,
        CancellationToken ct)
    {
        try
        {
            var result = await userService.RefreshTokenAsync(dto, ct);
            return TypedResults.Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Unauthorized();
        }
    }

    // ---------------- ME ----------------

    private static async Task<Results<Ok<UserResponseDto>, UnauthorizedHttpResult>> GetCurrentUserAsync(
        ClaimsPrincipal userPrincipal,
        IUserService userService,
        CancellationToken ct)
    {
        if (!TryGetUserId(userPrincipal, out var userId))
            return TypedResults.Unauthorized();

        var user = await userService.GetProfileByIdAsync(userId, ct);

        return user is null
            ? TypedResults.Unauthorized()
            : TypedResults.Ok(user);
    }

    private static async Task<IResult> GetByUsernameAsync(
    string username,
    IUserService userService,
    CancellationToken ct)
    {
        var user = await userService.GetProfileByUsernameAsync(username, ct);

        return user is null
            ? Results.NotFound()
            : Results.Ok(new { user.Id, user.Username });
    }

    // ---------------- UPDATE ----------------

    private static async Task<Results<Ok<UserResponseDto>, BadRequest<string>, Conflict<string>, UnauthorizedHttpResult>> UpdateProfileAsync(
        int id,
        uint clientRowVersion,
        UpdateUserDto dto,
        ClaimsPrincipal userPrincipal,
        IUserService userService,
        CancellationToken ct)
    {
        if (!TryGetUserId(userPrincipal, out var currentUserId) || currentUserId != id)
            return TypedResults.Unauthorized();

        try
        {
            var result = await userService.UpdateProfileAsync(id, dto, ct);
            return TypedResults.Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return TypedResults.BadRequest(ex.Message);
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

    // ---------------- DELETE ----------------

    private static async Task<Results<NoContent, UnauthorizedHttpResult>> DeleteAccountAsync(
        int id,
        ClaimsPrincipal userPrincipal,
        IUserService userService,
        CancellationToken ct)
    {
        if (!TryGetUserId(userPrincipal, out var currentUserId) || currentUserId != id)
            return TypedResults.Unauthorized();

        await userService.DeleteAccountAsync(id, ct);

        return TypedResults.NoContent();
    }

    private static bool TryGetUserId(ClaimsPrincipal user, out int userId)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out userId);
    }
}