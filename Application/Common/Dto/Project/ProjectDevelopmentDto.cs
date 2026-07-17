using ProjectPlanner.Application.Common.Dto.Development;

namespace ProjectPlanner.Application.Common.Dto.Project;

public record ProjectDevelopmentDto(
    int ProjectId,
    int LinkedRepositoryCount,
    IEnumerable<BranchDto> ActiveBranches,
    IEnumerable<IssuePullRequestDto> OpenPullRequests);
