namespace ProjectPlanner.Application.Common.Dtos.Development;

public record ConnectRepositoryDto(
    string Owner,
    string Repository
);