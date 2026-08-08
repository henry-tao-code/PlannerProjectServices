using ProjectPlanner.Application.Common.Dtos.Search;

namespace ProjectPlanner.Application.Common.Interfaces.AI;

public interface IVectorSearchService
{
    Task<IReadOnlyList<SearchResultDto>> SearchAsync(
        string query,
        int? projectId,
        int topK,
        CancellationToken cancellationToken = default);
}