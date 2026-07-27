using Microsoft.EntityFrameworkCore;
using ProjectPlanner.Application.Common.Dto.Search;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Infrastructure.Persistence.Repositories;

public class SearchQueryRepository(ProjectPlannerDbContext context) : ISearchQueryRepository
{
    public async Task<IReadOnlyList<SearchResultDto>> SearchAsync(
        int projectId,
        string query,
        CancellationToken cancellationToken = default)
    {
        var results = await context.SearchDocuments
            .AsNoTracking()
            .Where(x =>
                x.ProjectId == projectId &&
                (
                    EF.Functions.TrigramsAreSimilar(
                        x.Title,
                        query)
                    ||
                    EF.Functions.TrigramsAreSimilar(
                        x.Content,
                        query)
                ))
            .Select(x => new SearchResultDto
            {
                EntityId = x.EntityId,

                EntityType = x.EntityType.ToString(),

                ProjectId = x.ProjectId,

                Title = x.Title,

                Content = x.Content,

                Score =
                    Math.Max(
                        EF.Functions.TrigramsSimilarity(
                            x.Title,
                            query),

                        EF.Functions.TrigramsSimilarity(
                            x.Content,
                            query))
            })
            .OrderByDescending(x => x.Score)
            .Take(50)
            .ToListAsync(cancellationToken);


        return results;
    }
}