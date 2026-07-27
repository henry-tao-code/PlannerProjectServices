using ProjectPlanner.Application.Common.Dto.Search;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Application.Services.Implementations;

public class SearchQueryService(
    ISearchQueryRepository searchRepository) : ISearchQueryService
{
    public async Task<IReadOnlyList<SearchResultDto>> SearchAsync(
        int projectId,
        string query,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        return await searchRepository.SearchAsync(
            projectId,
            query,
            ct);
    }
}