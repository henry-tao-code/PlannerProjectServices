using ProjectPlanner.Application.Services;

namespace ProjectPlanner.Api.Endpoints;

public static class IssueAttachmentEndpoints
{
    public static IEndpointRouteBuilder MapIssueAttachmentEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/issues/{issueId:int}/attachments")
            .WithTags("Issue Attachments")
            .DisableAntiforgery();
        group.MapPost("/", UploadAttachmentAsync);
        group.MapGet("/", GetAttachmentsAsync);
        group.MapDelete("/{attachmentId:long}", DeleteAttachmentAsync);

        return app;
    }
    private static async Task<IResult> UploadAttachmentAsync(
    int issueId,
    IFormFile file,
    IIssueAttachmentService attachmentService,
    CancellationToken cancellationToken)
    {
        try
        {
            var attachment = await attachmentService.UploadAsync(
                issueId,
                file,
                cancellationToken);

            return Results.Created(
                $"/api/issues/{issueId}/attachments/{attachment.Id}",
                attachment);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
    }

    private static async Task<IResult> GetAttachmentsAsync(
        int issueId,
        IIssueAttachmentService attachmentService,
        CancellationToken cancellationToken)
    {
        var attachments =
            await attachmentService.GetByIssueIdAsync(
                issueId,
                cancellationToken);

        return Results.Ok(attachments);
    }

    private static async Task<IResult> DeleteAttachmentAsync(
        int issueId,
        long attachmentId,
        IIssueAttachmentService attachmentService,
        CancellationToken cancellationToken)
    {
        try
        {
            await attachmentService.DeleteAsync(
                attachmentId,
                cancellationToken);

            return Results.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
    }
}