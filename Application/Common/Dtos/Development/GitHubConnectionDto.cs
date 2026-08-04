namespace ProjectPlanner.Application.Common.Dtos.Development;

public record GitHubConnectionDto(
    int Id,
    long GithubUserId,
    string GithubUsername,
    DateTime ConnectedAt
);