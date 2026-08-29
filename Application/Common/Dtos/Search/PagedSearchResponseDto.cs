namespace ProjectPlanner.Application.Common.Dtos.Search;

public record PagedSearchResponseDto<T>(
    IEnumerable<T> Items,
    int Page,
    int PageSize
);
