namespace ProjectPlanner.Application.Common.Dto.Development;

public record DevelopmentSummaryDto(
    IReadOnlyList<IssueCommitDto> Commits,
    IReadOnlyList<IssuePullRequestDto> PullRequests
);