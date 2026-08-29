using ProjectPlanner.Application.Common.Dtos.Search;

namespace ProjectPlanner.Application.Services;

public interface ISearchService
{
    Task<PagedSearchResponseDto<SearchResultDto>> SearchAsync(
        SearchRequestDto request,
        CancellationToken cancellationToken = default);
}