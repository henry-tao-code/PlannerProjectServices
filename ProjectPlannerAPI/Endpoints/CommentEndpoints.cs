using ProjectPlanner.Application.Common.Dto.Comment;
using ProjectPlanner.Application.Services;

namespace ProjectPlanner.Api.Endpoints;

public static class CommentEndpoints
{
    public static IEndpointRouteBuilder MapCommentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/comments").WithTags("Comments");
        group.MapPost("/issue/{issueId:int}", CreateCommentAsync);
        group.MapGet("/issue/{issueId:int}", GetCommentsByIssueIdAsync);
        group.MapDelete("/{id:int}", DeleteCommentAsync);
        return app;
    }

    private static async Task<IResult> CreateCommentAsync(
    int issueId,
    CreateCommentDto dto,
    ICommentService commentService,
    CancellationToken cancellationToken)
    {
        try
        {
            var result = await commentService.CreateAsync(issueId, dto, cancellationToken);

            return Results.Created(
                $"/api/issues/{issueId}/comments/{result.Id}",
                result);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
    }

    private static async Task<IResult> GetCommentsByIssueIdAsync(
        int issueId,
        ICommentService commentService,
        CancellationToken cancellationToken)
    {
        try
        {
            var comments = await commentService.GetByIssueIdAsync(issueId, cancellationToken);
            return Results.Ok(comments);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
    }

    private static async Task<IResult> DeleteCommentAsync(
        int id,
        ICommentService commentService,
        CancellationToken cancellationToken)
    {
        var deleted = await commentService.DeleteAsync(id, cancellationToken);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}