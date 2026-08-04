namespace ProjectPlanner.Application.Common.Dtos.Development;

public record ProjectGitHubRepositoryDto(
    int Id,
    int ProjectId,
    string Owner,
    string Repository,
    string DefaultBranch
);