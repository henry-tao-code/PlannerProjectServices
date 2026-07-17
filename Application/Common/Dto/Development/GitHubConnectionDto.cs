namespace ProjectPlanner.Application.Common.Dto.Development;

public record GitHubConnectionDto(
    int Id,
    long GithubUserId,
    string GithubUsername,
    DateTime ConnectedAt
);