using ProjectPlanner.Application.Common.Dto.Development;

namespace ProjectPlanner.Application.Services;

public interface IGitHubApiClient
{
    Task<IEnumerable<GitHubApiCommitDto>> SearchCommitsAsync(
        string owner,
        string repository,
        string issueKey,
        CancellationToken ct = default);

    Task<IEnumerable<GitHubApiPullRequestDto>> SearchPullRequestsAsync(
        string owner,
        string repository,
        string issueKey,
        CancellationToken ct = default);
}
