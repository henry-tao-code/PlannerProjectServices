using ProjectPlanner.Application.Common.Dtos.Development;

namespace ProjectPlanner.Application.Common.Dtos.Project;

public record ProjectDevelopmentDto(
    int ProjectId,
    int LinkedRepositoryCount,
    IEnumerable<BranchDto> ActiveBranches,
    IEnumerable<IssuePullRequestDto> OpenPullRequests);
