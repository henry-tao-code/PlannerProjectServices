using ProjectPlanner.Application.Common.Dtos.Development;

namespace ProjectPlanner.Application.Services;

public interface IDevelopmentService
{
    Task<GitHubConnectionDto> ConnectGitHubAsync(
        int userId,
        string authorizationCode,
        CancellationToken ct = default);

    Task<ProjectGitHubRepositoryDto> ConnectRepositoryAsync(
        int projectId,
        string owner,
        string repository,
        CancellationToken ct = default);

    Task<IEnumerable<IssueCommitDto>> GetIssueCommitsAsync(
        int issueId,
        CancellationToken ct = default);

    Task<IEnumerable<IssuePullRequestDto>> GetIssuePullRequestsAsync(
        int issueId,
        CancellationToken ct = default);

    Task SyncIssueCommitsAsync(
        int issueId,
        CancellationToken ct = default);

    Task SyncIssuePullRequestsAsync(
        int issueId,
        CancellationToken ct = default);

    Task ProcessPushWebhookAsync(
        string owner,
        string repository,
        IEnumerable<GitHubWebhookCommitDto> commits,
        CancellationToken ct = default);

    Task ProcessPullRequestWebhookAsync(
        string owner,
        string repository,
        GitHubWebhookPullRequestDto prData,
        CancellationToken ct = default);
}