namespace ProjectPlanner.Application.Common.Dto.Development;

public record IssueCommitDto(
    int Id,
    string Sha,
    string Message,
    string Author,
    string Url,
    DateTime CommittedAt
);