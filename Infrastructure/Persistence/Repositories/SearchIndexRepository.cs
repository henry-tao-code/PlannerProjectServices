using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Infrastructure.Persistence.Repositories;

public class SearchIndexRepository(ProjectPlannerDbContext dbContext)
    : ISearchIndexRepository
{
    public async Task<SearchDocument?> GetAsync(
        SearchEntityType type,
        int entityId,
        CancellationToken ct)
    {
        return await dbContext.SearchDocuments
            .FirstOrDefaultAsync(x =>
                x.EntityType == type &&
                x.EntityId == entityId,
                ct);
    }


    public async Task AddAsync(
        SearchDocument document,
        CancellationToken ct)
    {
        await dbContext.SearchDocuments.AddAsync(document, ct);
    }


    public Task UpdateAsync(
        SearchDocument document,
        CancellationToken ct)
    {
        dbContext.SearchDocuments.Update(document);

        return Task.CompletedTask;
    }


    public Task DeleteAsync(
        SearchDocument document,
        CancellationToken ct)
    {
        dbContext.SearchDocuments.Remove(document);

        return Task.CompletedTask;
    }
}