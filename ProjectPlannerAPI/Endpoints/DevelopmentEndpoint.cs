using Microsoft.AspNetCore.Mvc;
using ProjectPlanner.Application.Common.Dto.Development;
using ProjectPlanner.Application.Common.Interfaces.Services;
using System.Text.Json;

namespace ProjectPlanner.Api.Endpoints;

public static class DevelopmentEndpoints
{
    public static void MapDevelopmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/development").WithTags("Development");

        group.MapPost("/projects/{projectId:int}/connect-repository", async (
            int projectId,
            [FromBody] ConnectRepositoryRequest request,
            IDevelopmentService developmentService,
            CancellationToken ct) =>
        {
            if (request == null)
                return Results.BadRequest("Request body is required.");

            if (string.IsNullOrWhiteSpace(request.Owner) || string.IsNullOrWhiteSpace(request.Repository))
                return Results.BadRequest("Owner and Repository are required.");

            try
            {
                var result = await developmentService.ConnectRepositoryAsync(
                    projectId,
                    request.Owner.Trim(),
                    request.Repository.Trim(),
                    ct);
                return Results.Ok(result);
            }
            catch (KeyNotFoundException knf)
            {
                return Results.NotFound(knf.Message);
            }
        });

        group.MapGet("/issues/{issueId:int}/commits", async (
            int issueId,
            IDevelopmentService developmentService,
            CancellationToken ct) =>
        {
            var commits = await developmentService.GetIssueCommitsAsync(issueId, ct);
            return Results.Ok(commits);
        });

        group.MapGet("/issues/{issueId:int}/pull-requests", async (
            int issueId,
            IDevelopmentService developmentService,
            CancellationToken ct) =>
        {
            var prs = await developmentService.GetIssuePullRequestsAsync(issueId, ct);
            return Results.Ok(prs);
        });

        group.MapPost("/issues/{issueId:int}/sync-commits", async (
            int issueId,
            IDevelopmentService developmentService,
            CancellationToken ct) =>
        {
            await developmentService.SyncIssueCommitsAsync(issueId, ct);
            return Results.NoContent();
        });

        group.MapPost("/issues/{issueId:int}/sync-pull-requests", async (
            int issueId,
            IDevelopmentService developmentService,
            CancellationToken ct) =>
        {
            await developmentService.SyncIssuePullRequestsAsync(issueId, ct);
            return Results.NoContent();
        });

        group.MapPost("/webhook", async (
            HttpRequest request,
            [FromBody] JsonElement payload,
            IDevelopmentService developmentService,
            CancellationToken ct) =>
        {
            // 1. Identify the event type from GitHub's header
            if (!request.Headers.TryGetValue("X-GitHub-Event", out var eventName))
            {
                return Results.BadRequest("Missing X-GitHub-Event header.");
            }

            // 2. Route the payload to the correct service method
            try
            {
                switch (eventName.ToString().ToLower())
                {
                    case "push":
                        await HandlePushEventAsync(payload, developmentService, ct);
                        break;
                    case "pull_request":
                        await HandlePullRequestEventAsync(payload, developmentService, ct);
                        break;
                    case "ping":
                        return Results.Ok(new { message = "Pong" });
                    default:
                        return Results.BadRequest("Unsupported GitHub event.");
                }
            }
            catch (Exception)
            {
                // In production, log the exception here
                return Results.Problem("Error processing webhook payload.", statusCode: 500);
            }

            return Results.Ok();
        });
    }

    private static async Task HandlePushEventAsync(
        JsonElement payload,
        IDevelopmentService service,
        CancellationToken ct)
    {
        if (!payload.TryGetProperty("repository", out var repo)) return;

        string? owner = null;
        if (repo.TryGetProperty("owner", out var ownerProp))
        {
            owner = ownerProp.TryGetProperty("login", out var login) ? login.GetString() :
                    ownerProp.TryGetProperty("name", out var name) ? name.GetString() : null;
        }

        var repoName = repo.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : null;

        if (!payload.TryGetProperty("commits", out var commitsArray)) return;

        var commits = commitsArray.EnumerateArray().Select(c =>
        {
            var id = c.TryGetProperty("id", out var idProp) ? idProp.GetString() ?? string.Empty : string.Empty;
            var message = c.TryGetProperty("message", out var mProp) ? mProp.GetString() ?? string.Empty : string.Empty;
            var url = c.TryGetProperty("url", out var urlProp) ? urlProp.GetString() ?? string.Empty : string.Empty;
            DateTime ts = DateTime.UtcNow;
            if (c.TryGetProperty("timestamp", out var tsProp) && tsProp.ValueKind == JsonValueKind.String)
            {
                if (tsProp.TryGetDateTimeOffset(out var dto)) ts = dto.UtcDateTime;
            }

            var authorName = "Unknown";
            if (c.TryGetProperty("author", out var authorProp))
            {
                authorName = authorProp.TryGetProperty("name", out var aName) ? aName.GetString() ?? "Unknown" : "Unknown";
            }

            return new GitHubWebhookCommitDto
            {
                Id = id,
                Message = message,
                Url = url,
                Timestamp = ts,
                Author = new GitHubWebhookAuthorDto { Name = authorName }
            };
        });

        if (!string.IsNullOrWhiteSpace(owner) && !string.IsNullOrWhiteSpace(repoName))
        {
            await service.ProcessPushWebhookAsync(owner!, repoName!, commits, ct);
        }
    }

    private static async Task HandlePullRequestEventAsync(
        JsonElement payload,
        IDevelopmentService service,
        CancellationToken ct)
    {
        if (!payload.TryGetProperty("repository", out var repo)) return;

        var owner = repo.TryGetProperty("owner", out var ownerProp) && ownerProp.TryGetProperty("login", out var loginProp)
            ? loginProp.GetString()
            : null;

        var repoName = repo.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : null;

        if (!payload.TryGetProperty("pull_request", out var pr)) return;

        var prData = new GitHubWebhookPullRequestDto
        {
            Number = pr.TryGetProperty("number", out var numberProp) && numberProp.TryGetInt32(out var n) ? n : 0,
            Title = pr.TryGetProperty("title", out var titleProp) ? titleProp.GetString() ?? string.Empty : string.Empty,
            Url = pr.TryGetProperty("html_url", out var urlProp) ? urlProp.GetString() ?? string.Empty : string.Empty,
            State = pr.TryGetProperty("state", out var stateProp) ? stateProp.GetString() ?? string.Empty : string.Empty,
            Merged = pr.TryGetProperty("merged", out var mergedProp) && mergedProp.GetBoolean(),
            BranchName = pr.TryGetProperty("head", out var headProp) && headProp.TryGetProperty("ref", out var refProp) ? refProp.GetString() ?? string.Empty : string.Empty
        };

        if (!string.IsNullOrWhiteSpace(owner) && !string.IsNullOrWhiteSpace(repoName))
        {
            await service.ProcessPullRequestWebhookAsync(owner!, repoName!, prData, ct);
        }
    }
}

// Request DTO
public class ConnectRepositoryRequest
{
    public string Owner { get; set; } = string.Empty;
    public string Repository { get; set; } = string.Empty;
}