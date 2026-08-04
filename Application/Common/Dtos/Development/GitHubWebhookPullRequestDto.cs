namespace ProjectPlanner.Application.Common.Dtos.Development;

public record GitHubWebhookPullRequestDto
{
    public long GithubPullRequestId { get; init; }
    public int Number { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public bool Merged { get; init; }
    public string BranchName { get; init; } = string.Empty;
}