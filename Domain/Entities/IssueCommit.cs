namespace Domain.Entities;

public class IssueCommit
{
    private IssueCommit() { }

    public int Id { get; private set; }

    public int IssueId { get; private set; }
    public Issue Issue { get; private set; } = null!;

    public string Sha { get; private set; } = null!;

    public string Message { get; private set; } = null!;

    public string Author { get; private set; } = null!;

    public string Url { get; private set; } = null!;

    public DateTime CommittedAt { get; private set; }

    public static IssueCommit Create(
        int issueId,
        string sha,
        string message,
        string author,
        string url,
        DateTime committedAt)
    {
        return new IssueCommit
        {
            IssueId = issueId,
            Sha = sha,
            Message = message,
            Author = author,
            Url = url,
            CommittedAt = committedAt
        };
    }
}