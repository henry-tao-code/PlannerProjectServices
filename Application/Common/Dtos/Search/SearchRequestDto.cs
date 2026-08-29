namespace ProjectPlanner.Application.Common.Dtos.Search;

public record SearchRequestDto(
    string? Query = null,
    int? ProjectId = null,
    string? Status = null,
    int Page = 1,
    int PageSize = 20
);