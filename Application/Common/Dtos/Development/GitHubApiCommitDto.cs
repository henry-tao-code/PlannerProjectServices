namespace ProjectPlanner.Application.Common.Dtos.Development;

public record GitHubApiCommitDto
{
    public string Sha { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string AuthorName { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public DateTime CommittedAt { get; init; }
}