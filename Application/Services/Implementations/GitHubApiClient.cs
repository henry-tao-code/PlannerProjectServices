using Octokit;
using ProjectPlanner.Application.Common.Dto.Development;

namespace ProjectPlanner.Application.Services.Implementations;

public class GitHubApiClient : IGitHubApiClient
{
    private readonly IGitHubClient _client;

    public GitHubApiClient()
    {
        _client = new GitHubClient(new ProductHeaderValue("ProjectPlannerApp"));
    }

    public async Task<IEnumerable<GitHubApiCommitDto>> SearchCommitsAsync(
        string owner,
        string repository,
        string issueKey,
        CancellationToken ct = default)
    {
        // 1. Construct the raw search query parameters
        var parameters = new Dictionary<string, string>
        {
            { "q", $"{issueKey} repo:{owner}/{repository}" }
        };

        // 2. Call the GitHub Search Commits API directly using Octokit's connection.
        // We use the specific preview Accept header that GitHub requires for commit searches.
        var response = await _client.Connection.Get<GitHubCommitSearchResult>(
            new Uri("search/commits", UriKind.Relative),
            parameters,
            "application/vnd.github.cloak-preview+json");

        // 3. Map the raw results to your DTO
        return response.Body.Items.Select(c => new GitHubApiCommitDto
        {
            Sha = c.Sha,
            Message = c.Commit.Message,
            AuthorName = c.Commit.Author.Name,
            Url = c.HtmlUrl,
            CommittedAt = c.Commit.Author.Date.UtcDateTime
        });
    }

    public async Task<IEnumerable<GitHubApiPullRequestDto>> SearchPullRequestsAsync(
        string owner,
        string repository,
        string issueKey,
        CancellationToken ct = default)
    {
        // This one IS supported natively by Octokit's SearchClient
        var request = new SearchIssuesRequest(issueKey)
        {
            Repos = new RepositoryCollection { { owner, repository } },
            Type = IssueTypeQualifier.PullRequest
        };

        var searchResult = await _client.Search.SearchIssues(request);

        return searchResult.Items.Select(pr => new GitHubApiPullRequestDto
        {
            Number = pr.Number,
            Title = pr.Title,
            Url = pr.HtmlUrl,
            State = pr.State.StringValue,
            IsMerged = pr.State.Value == ItemState.Closed // Fallback logic as Search API doesn't return exact merge status
        });
    }
}
public class GitHubCommitSearchResult
{
    public IReadOnlyList<GitHubCommit> Items { get; set; } = new List<GitHubCommit>();
}