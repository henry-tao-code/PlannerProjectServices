using Domain.Entities;
using ProjectPlanner.Application.Common.Dto.Development;
using ProjectPlanner.Application.Common.Interfaces.Persistence;
using ProjectPlanner.Application.Common.Interfaces.Services;
using System.Text.RegularExpressions;

namespace ProjectPlanner.Application.Services.Implementations;

public class DevelopmentService(
    IDevelopmentRepository developmentRepository,
    IProjectRepository projectRepository,
    IIssueRepository issueRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IGitHubApiClient githubApiClient) : IDevelopmentService
{
    private readonly IDevelopmentRepository _developmentRepository = developmentRepository;
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IIssueRepository _issueRepository = issueRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IGitHubApiClient _githubApiClient = githubApiClient;

    // ==========================================
    // Connection & Querying Methods
    // ==========================================

    public async Task<GitHubConnectionDto> ConnectGitHubAsync(
        int userId,
        string authorizationCode,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(authorizationCode))
            throw new ArgumentException("authorizationCode is required.", nameof(authorizationCode));

        User user = await _userRepository.GetByIdAsync(userId, ct)
            ?? throw new KeyNotFoundException("User not found.");

        // Try to find an existing connection for the user
        var existing = await _developmentRepository.GetConnectionByUserIdAsync(userId, ct);

        // Note: Ideally we would exchange the authorization code for an access token
        // and fetch the GitHub user id and username via the injected IGitHubApiClient.
        // Because the API client interface may vary, this implementation stores
        // the provided authorizationCode as the access token placeholder and
        // creates a minimal connection record. Replace with proper OAuth exchange
        // when integrating a concrete GitHub client.

        if (existing != null)
        {
            existing.UpdateAccessToken(authorizationCode);
            await _developmentRepository.UpdateConnectionAsync(existing, ct);
        }
        else
        {
            var connection = GitHubConnection.Create(
                userId,
                githubUserId: 0,
                githubUsername: "unknown",
                accessToken: authorizationCode);

            await _developmentRepository.AddConnectionAsync(connection, ct);
            existing = connection;
        }

        await _unitOfWork.SaveChangesAsync(ct);

        return new GitHubConnectionDto(
            existing.Id,
            existing.GithubUserId,
            existing.GithubUsername,
            existing.ConnectedAt);
    }

    public async Task<ProjectGitHubRepositoryDto> ConnectRepositoryAsync(
        int projectId,
        string owner,
        string repository,
        CancellationToken ct = default)
    {
        Project project = await _projectRepository.GetByIdAsync(projectId, ct)
            ?? throw new KeyNotFoundException("Project not found.");

        long githubRepositoryId = 0; // Usually fetched from GitHub API during connection

        var githubRepository = ProjectGitHubRepository.Create(
            projectId,
            owner,
            repository,
            "main",
            githubRepositoryId);

        await _developmentRepository.AddRepositoryAsync(githubRepository, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new ProjectGitHubRepositoryDto(
            githubRepository.Id,
            githubRepository.ProjectId,
            githubRepository.Owner,
            githubRepository.Repository,
            githubRepository.DefaultBranch);
    }

    public async Task<IEnumerable<IssueCommitDto>> GetIssueCommitsAsync(
        int issueId,
        CancellationToken ct = default)
    {
        var commits = await _developmentRepository.GetCommitsByIssueIdAsync(issueId, ct);

        return commits.Select(c =>
            new IssueCommitDto(
                c.Id,
                c.Sha,
                c.Message,
                c.Author,
                c.Url,
                c.CommittedAt));
    }

    public async Task<IEnumerable<IssuePullRequestDto>> GetIssuePullRequestsAsync(
        int issueId,
        CancellationToken ct = default)
    {
        var prs = await _developmentRepository.GetPullRequestsByIssueIdAsync(issueId, ct);

        return prs.Select(pr =>
            new IssuePullRequestDto(
                pr.Id,
                pr.Number,
                pr.Title,
                pr.Url,
                pr.State,
                pr.Merged));
    }

    public async Task SyncIssueCommitsAsync(int issueId, CancellationToken ct = default)
    {
        var issue = await _issueRepository.GetByIdAsync(issueId, ct)
            ?? throw new KeyNotFoundException("Issue not found.");

        var githubRepo = await _developmentRepository.GetRepositoryByProjectIdAsync(issue.ProjectId, ct)
            ?? throw new InvalidOperationException("No GitHub repository connected to this project.");

        var githubCommits = await _githubApiClient.SearchCommitsAsync(
            githubRepo.Owner,
            githubRepo.Repository,
            issue.IssueKey,
            ct);

        var existingCommits = await _developmentRepository.GetCommitsByIssueIdAsync(issueId, ct);
        var existingShas = existingCommits.Select(c => c.Sha).ToHashSet();

        foreach (var gc in githubCommits)
        {
            if (!existingShas.Contains(gc.Sha))
            {
                var newCommit = IssueCommit.Create(
                    issueId,
                    gc.Sha,
                    gc.Message,
                    gc.AuthorName,
                    gc.Url,
                    gc.CommittedAt);

                await _developmentRepository.AddCommitAsync(newCommit, ct);
            }
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task SyncIssuePullRequestsAsync(int issueId, CancellationToken ct = default)
    {
        var issue = await _issueRepository.GetByIdAsync(issueId, ct)
            ?? throw new KeyNotFoundException("Issue not found.");

        var githubRepo = await _developmentRepository.GetRepositoryByProjectIdAsync(issue.ProjectId, ct)
            ?? throw new InvalidOperationException("No GitHub repository connected.");

        var githubPrs = await _githubApiClient.SearchPullRequestsAsync(
            githubRepo.Owner,
            githubRepo.Repository,
            issue.IssueKey,
            ct);

        var existingPrs = await _developmentRepository.GetPullRequestsByIssueIdAsync(issueId, ct);
        var existingPrNumbers = existingPrs.ToDictionary(pr => pr.Number);

        foreach (var gPr in githubPrs)
        {
            if (existingPrNumbers.TryGetValue(gPr.Number, out var existingPr))
            {
                existingPr.UpdateState(gPr.State, gPr.IsMerged);
            }
            else
            {
                var newPr = IssuePullRequest.Create(
                    issueId,
                    gPr.GithubPullRequestId, // Pass the ID now
                    gPr.Number,
                    gPr.Title,
                    gPr.Url,
                    gPr.State,
                    gPr.IsMerged);

                await _developmentRepository.AddPullRequestAsync(newPr, ct);
            }
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }

    // ==========================================
    // Webhook Processing Methods (Push from GitHub)
    // ==========================================

    public async Task ProcessPushWebhookAsync(
        string owner,
        string repository,
        IEnumerable<GitHubWebhookCommitDto> commits,
        CancellationToken ct = default)
    {
        var githubRepo = await _developmentRepository.GetRepositoryByOwnerAndNameAsync(owner, repository, ct);
        if (githubRepo == null) return;

        foreach (var commit in commits)
        {
            var issueKeys = ExtractIssueKeys(commit.Message);

            foreach (var key in issueKeys)
            {
                var issue = await _issueRepository.GetByKeyAndProjectIdAsync(key, githubRepo.ProjectId, ct);
                if (issue == null) continue;

                var exists = await _developmentRepository.CommitExistsAsync(commit.Id, ct);
                if (!exists)
                {
                    var newCommit = IssueCommit.Create(
                        issue.Id,
                        commit.Id,
                        commit.Message,
                        commit.Author.Name,
                        commit.Url,
                        commit.Timestamp);

                    await _developmentRepository.AddCommitAsync(newCommit, ct);
                }
            }
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task ProcessPullRequestWebhookAsync(
        string owner,
        string repository,
        GitHubWebhookPullRequestDto prData,
        CancellationToken ct = default)
    {
        var githubRepo = await _developmentRepository.GetRepositoryByOwnerAndNameAsync(owner, repository, ct);
        if (githubRepo == null) return;

        var textToSearch = $"{prData.Title} {prData.BranchName}";
        var issueKeys = ExtractIssueKeys(textToSearch);

        foreach (var key in issueKeys)
        {
            var issue = await _issueRepository.GetByKeyAndProjectIdAsync(key, githubRepo.ProjectId, ct);
            if (issue == null) continue;

            var existingPr = await _developmentRepository.GetPullRequestByNumberAsync(issue.Id, prData.Number, ct);

            if (existingPr != null)
            {
                existingPr.UpdateState(prData.State, prData.Merged);
            }
            else
            {
                var newPr = IssuePullRequest.Create(
                    issue.Id,
                    prData.GithubPullRequestId, // Pass the ID now
                    prData.Number,
                    prData.Title,
                    prData.Url,
                    prData.State,
                    prData.Merged);

                await _developmentRepository.AddPullRequestAsync(newPr, ct);
            }
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }

    private IEnumerable<string> ExtractIssueKeys(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return [];

        // Matches standard Jira keys: 1-10 uppercase letters, a dash, and digits (e.g., ENG-402)
        var regex = new Regex(@"[A-Z]{1,10}-\d+", RegexOptions.Compiled);
        var matches = regex.Matches(text);

        return matches.Select(m => m.Value).Distinct();
    }
}