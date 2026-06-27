using Domain.Enums;

namespace ProjectPlanner.Application.Common.Dto.Development;

public record PullRequestDto(
    int Id,
    string Title,
    string Url,
    PullRequestStatus Status,
    string SourceBranch,
    string TargetBranch);