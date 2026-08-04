namespace ProjectPlanner.Application.Common.Dtos.Development;

public record BranchDto
{
    public string Name { get; init; } = string.Empty;
    public string CommitSha { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public bool IsProtected { get; init; }
}