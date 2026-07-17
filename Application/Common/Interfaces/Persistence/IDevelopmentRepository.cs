using Domain.Entities;

namespace ProjectPlanner.Application.Common.Interfaces.Persistence;

public interface IDevelopmentRepository
{
    Task<GitHubConnection?> GetConnectionByUserIdAsync(int userId, CancellationToken ct = default);
    Task AddConnectionAsync(GitHubConnection connection, CancellationToken ct = default);

    Task<ProjectGitHubRepository?> GetRepositoryByProjectIdAsync(int projectId, CancellationToken ct = default);
    Task AddRepositoryAsync(ProjectGitHubRepository repository, CancellationToken ct = default);

    Task<ProjectGitHubRepository?> GetRepositoryByOwnerAndNameAsync(string owner, string repositoryName, CancellationToken ct = default);

    Task UpdateConnectionAsync(GitHubConnection connection, CancellationToken ct = default);

    Task AddCommitAsync(IssueCommit commit, CancellationToken ct = default);
    Task<IReadOnlyList<IssueCommit>> GetCommitsByIssueIdAsync(int issueId, CancellationToken ct = default);

    Task<bool> CommitExistsAsync(string sha, CancellationToken ct = default);

    Task AddPullRequestAsync(IssuePullRequest pullRequest, CancellationToken ct = default);
    Task<IReadOnlyList<IssuePullRequest>> GetPullRequestsByIssueIdAsync(int issueId, CancellationToken ct = default);

    Task<IssuePullRequest?> GetPullRequestByNumberAsync(int issueId, int number, CancellationToken ct = default);
}