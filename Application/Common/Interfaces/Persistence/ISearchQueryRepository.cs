using ProjectPlanner.Application.Common.Dtos.Search;

namespace ProjectPlanner.Application.Common.Interfaces.Persistence;

public interface ISearchQueryRepository
{
    Task<IReadOnlyList<SearchResultDto>> SearchAsync(
        int projectId,
        string query,
        CancellationToken cancellationToken = default);
}