namespace ProjectPlanner.Application.Common.Dtos.Development;

public record DevelopmentSummaryDto(
    IReadOnlyList<IssueCommitDto> Commits,
    IReadOnlyList<IssuePullRequestDto> PullRequests
);