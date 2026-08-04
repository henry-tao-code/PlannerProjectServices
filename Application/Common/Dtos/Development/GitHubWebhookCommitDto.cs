namespace ProjectPlanner.Application.Common.Dtos.Development;

public record GitHubWebhookCommitDto
{
    public string Id { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public GitHubWebhookAuthorDto Author { get; init; } = new();
}