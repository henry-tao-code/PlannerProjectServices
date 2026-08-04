using ProjectPlanner.Application.Common.Dtos.Search;

namespace ProjectPlanner.Application.Services;

public interface ISearchQueryService
{
    Task<IReadOnlyList<SearchResultDto>> SearchAsync(
        int projectId,
        string query,
        CancellationToken ct);
}