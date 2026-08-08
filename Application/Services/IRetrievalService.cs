using Domain.Entities;
using ProjectPlanner.Application.Common.Dtos.Search;

namespace ProjectPlanner.Application.Services;

public interface IRetrievalService
{
    Task<IReadOnlyList<SearchResultDto>> SearchAsync(
        string query,
        QueryRoute route,
        int? projectId,
        CancellationToken cancellationToken);
}