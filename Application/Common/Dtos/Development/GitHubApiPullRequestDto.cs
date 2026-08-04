namespace ProjectPlanner.Application.Common.Dtos.Development;

public record GitHubApiPullRequestDto
{
    public long GithubPullRequestId { get; init; }
    public int Number { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public bool IsMerged { get; init; }
}
