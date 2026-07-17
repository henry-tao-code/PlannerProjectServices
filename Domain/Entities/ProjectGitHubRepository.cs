namespace Domain.Entities;

public class ProjectGitHubRepository
{
    private ProjectGitHubRepository() { }

    public int Id { get; private set; }

    public int ProjectId { get; private set; }
    public Project Project { get; private set; } = null!;

    public string Owner { get; private set; } = null!;

    public string Repository { get; private set; } = null!;

    public string DefaultBranch { get; private set; } = "main";

    public long GithubRepositoryId { get; private set; }

    public DateTime ConnectedAt { get; private set; }

    public static ProjectGitHubRepository Create(
        int projectId,
        string owner,
        string repository,
        string defaultBranch,
        long githubRepositoryId)
    {
        return new ProjectGitHubRepository
        {
            ProjectId = projectId,
            Owner = owner,
            Repository = repository,
            DefaultBranch = defaultBranch,
            GithubRepositoryId = githubRepositoryId,
            ConnectedAt = DateTime.UtcNow
        };
    }

    public void ChangeDefaultBranch(string branch)
    {
        DefaultBranch = branch;
    }
}