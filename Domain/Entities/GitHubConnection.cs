namespace Domain.Entities;

public class GitHubConnection
{
    private GitHubConnection() { }

    public int Id { get; private set; }

    public int UserId { get; private set; }
    public User User { get; private set; } = null!;

    public long GithubUserId { get; private set; }

    public string GithubUsername { get; private set; } = null!;

    public string AccessToken { get; private set; } = null!;

    public DateTime ConnectedAt { get; private set; }

    public static GitHubConnection Create(
        int userId,
        long githubUserId,
        string githubUsername,
        string accessToken)
    {
        return new GitHubConnection
        {
            UserId = userId,
            GithubUserId = githubUserId,
            GithubUsername = githubUsername,
            AccessToken = accessToken,
            ConnectedAt = DateTime.UtcNow
        };
    }

    public void UpdateAccessToken(string accessToken)
    {
        AccessToken = accessToken;
    }
}