using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Infrastructure.Persistence.Repositories;

public class DevelopmentRepository(ProjectPlannerDbContext dbContext) : IDevelopmentRepository
{
    private readonly ProjectPlannerDbContext _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<GitHubConnection?> GetConnectionByUserIdAsync(
        int userId,
        CancellationToken ct = default)
    {
        // Return a tracked entity so callers can update it if necessary.
        return await _context.GitHubConnections
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);
    }

    public async Task AddConnectionAsync(
        GitHubConnection connection,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(connection);

        await _context.GitHubConnections.AddAsync(connection, ct);
    }

    public async Task UpdateConnectionAsync(GitHubConnection connection, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(connection);

        _context.GitHubConnections.Update(connection);
        await Task.CompletedTask;
    }

    public async Task<ProjectGitHubRepository?> GetRepositoryByProjectIdAsync(
        int projectId,
        CancellationToken ct = default)
    {
        return await _context.GitHubRepositories
            .AsNoTracking()
            .Include(x => x.Project)
            .FirstOrDefaultAsync(
                x => x.ProjectId == projectId,
                ct);
    }

    public async Task AddRepositoryAsync(
        ProjectGitHubRepository repository,
        CancellationToken ct = default)
    {
        await _context.GitHubRepositories.AddAsync(repository, ct);
    }

    public async Task AddCommitAsync(
        IssueCommit commit,
        CancellationToken ct = default)
    {
        await _context.IssueCommits.AddAsync(commit, ct);
    }

    public async Task<IReadOnlyList<IssueCommit>> GetCommitsByIssueIdAsync(
        int issueId,
        CancellationToken ct = default)
    {
        return await _context.IssueCommits
            .AsNoTracking()
            .Where(x => x.IssueId == issueId)
            .OrderByDescending(x => x.CommittedAt)
            .ToListAsync(ct);
    }

    public async Task AddPullRequestAsync(
        IssuePullRequest pullRequest,
        CancellationToken ct = default)
    {
        await _context.IssuePullRequests.AddAsync(
            pullRequest,
            ct);
    }

    public async Task<IReadOnlyList<IssuePullRequest>> GetPullRequestsByIssueIdAsync(
        int issueId,
        CancellationToken ct = default)
    {
        return await _context.IssuePullRequests
            .AsNoTracking()
            .Where(x => x.IssueId == issueId)
            .OrderByDescending(x => x.Number)
            .ToListAsync(ct);
    }

    public async Task<ProjectGitHubRepository?> GetRepositoryByOwnerAndNameAsync(
        string owner,
        string repositoryName,
        CancellationToken ct = default)
    {
        return await _context.GitHubRepositories
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Owner == owner && x.Repository == repositoryName,
                ct);
    }

    public async Task<bool> CommitExistsAsync(
        string sha,
        CancellationToken ct = default)
    {
        return await _context.IssueCommits
            .AnyAsync(x => x.Sha == sha, ct);
    }

    public async Task<IssuePullRequest?> GetPullRequestByNumberAsync(
        int issueId,
        int number,
        CancellationToken ct = default)
    {
        return await _context.IssuePullRequests
            .FirstOrDefaultAsync(
                x => x.IssueId == issueId && x.Number == number,
                ct);
    }
}