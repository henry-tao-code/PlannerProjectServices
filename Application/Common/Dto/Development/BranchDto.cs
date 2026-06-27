namespace ProjectPlanner.Application.Common.Dto.Development;

public record BranchDto(
    string Name,
    string LastCommitHash,
    string Author,
    DateTime DispatchedAt);
