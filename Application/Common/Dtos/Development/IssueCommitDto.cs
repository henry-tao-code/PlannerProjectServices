namespace ProjectPlanner.Application.Common.Dtos.Development;

public record IssueCommitDto(
    int Id,
    string Sha,
    string Message,
    string Author,
    string Url,
    DateTime CommittedAt
);