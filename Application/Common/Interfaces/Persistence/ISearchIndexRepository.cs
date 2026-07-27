using Domain.Entities;
using Domain.Enums;

namespace ProjectPlanner.Application.Common.Interfaces.Persistence;

public interface ISearchIndexRepository
{
    Task<SearchDocument?> GetAsync(
        SearchEntityType type,
        int entityId,
        CancellationToken ct);

    Task AddAsync(
        SearchDocument document,
        CancellationToken ct);

    Task UpdateAsync(
        SearchDocument document,
        CancellationToken ct);

    Task DeleteAsync(
        SearchDocument document,
        CancellationToken ct);
}
