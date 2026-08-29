using Microsoft.EntityFrameworkCore;
using ProjectPlanner.Application.Common.Dtos.Search;
using ProjectPlanner.Application.Common.Interfaces.AI;
using ProjectPlanner.Infrastructure.Persistence;

namespace ProjectPlanner.Infrastructure.AI;

public class KeywordSearchService(
    IDbContextFactory<ProjectPlannerDbContext> contextFactory) : IKeywordSearchService
{
    public async Task<IReadOnlyList<SearchResultDto>> SearchAsync(
        string query,
        int? projectId,
        int topK,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return [];
        }

        var searchQuery = query.Trim();

        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);

        var queryable = dbContext.DocumentChunks
            .AsNoTracking()
            .Where(x => x.Content != null);

        if (projectId.HasValue)
        {
            queryable = queryable.Where(
                x => x.Attachment.Issue.ProjectId == projectId.Value);
        }

        var results = await queryable
            .Where(x =>
                EF.Functions.ToTsVector(
                    "english",
                    x.Content)
                .Matches(
                    EF.Functions.WebSearchToTsQuery(
                        "english",
                        searchQuery)))
            .Select(x => new
            {
                EntityId = x.Id,
                EntityType = "Document",
                x.Attachment.Issue.ProjectId,
                IssueId = (int?)x.Attachment.IssueId,
                Title = x.Attachment.OriginalFileName,
                x.Content,

                Rank =
                    EF.Functions.ToTsVector(
                        "english",
                        x.Content)
                    .Rank(
                        EF.Functions.WebSearchToTsQuery(
                            "english",
                            searchQuery))
            })
            .OrderByDescending(x => x.Rank)
            .Take(topK)
            .ToListAsync(cancellationToken);

        return [.. results
            .Select(x => new SearchResultDto(
                x.EntityId, x.EntityType, string.Empty, x.Title, x.Content,
                x.ProjectId, null, null, null, null, x.Rank))];
    }
}
