namespace ProjectPlanner.Application.Common.Dto.Development;

public record ConnectRepositoryRequest
{
    public string Owner { get; init; } = string.Empty;
    public string Repository { get; init; } = string.Empty;
}