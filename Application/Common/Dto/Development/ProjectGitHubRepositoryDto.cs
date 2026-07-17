namespace ProjectPlanner.Application.Common.Dto.Development;

public record ProjectGitHubRepositoryDto(
    int Id,
    int ProjectId,
    string Owner,
    string Repository,
    string DefaultBranch
);