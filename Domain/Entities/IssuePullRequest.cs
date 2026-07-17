namespace Domain.Entities;

public class IssuePullRequest
{
    private IssuePullRequest() { }

    public int Id { get; private set; }

    public int IssueId { get; private set; }
    public Issue Issue { get; private set; } = null!;

    public long GithubPullRequestId { get; private set; }

    public int Number { get; private set; }

    public string Title { get; private set; } = null!;

    public string Url { get; private set; } = null!;

    public string State { get; private set; } = null!;

    public bool Merged { get; private set; }

    public static IssuePullRequest Create(
        int issueId,
        long githubPullRequestId,
        int number,
        string title,
        string url,
        string state,
        bool merged)
    {
        return new IssuePullRequest
        {
            IssueId = issueId,
            GithubPullRequestId = githubPullRequestId,
            Number = number,
            Title = title,
            Url = url,
            State = state,
            Merged = merged
        };
    }

    public void UpdateState(string state, bool merged)
    {
        State = state;
        Merged = merged;
    }
}