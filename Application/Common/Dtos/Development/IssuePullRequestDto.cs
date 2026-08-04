namespace ProjectPlanner.Application.Common.Dtos.Development;

public record IssuePullRequestDto(
    int Id,
    int Number,
    string Title,
    string Url,
    string State,
    bool Merged
);