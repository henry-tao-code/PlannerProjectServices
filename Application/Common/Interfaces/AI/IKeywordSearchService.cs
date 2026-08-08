using ProjectPlanner.Application.Common.Dtos.Search;

namespace ProjectPlanner.Application.Common.Interfaces.AI;

public interface IKeywordSearchService
{
    Task<IReadOnlyList<SearchResultDto>> SearchAsync(
        string query,
        int? projectId,
        int topK,
        CancellationToken cancellationToken = default);
}