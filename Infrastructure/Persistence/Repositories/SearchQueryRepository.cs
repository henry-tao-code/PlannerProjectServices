using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
using ProjectPlanner.Application.Common.Dtos.Search;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Infrastructure.Persistence.Repositories;

public class SearchQueryRepository(ProjectPlannerDbContext context) : ISearchQueryRepository
{
    public async Task<IReadOnlyList<SearchResultDto>> SearchAsync(
    int projectId,
    string query,
    CancellationToken cancellationToken = default)
    {
        var indexedResults = await context.SearchDocuments
            .AsNoTracking()
            .Where(x =>
                x.ProjectId == projectId &&
                (
                    EF.Functions.TrigramsAreSimilar(x.Title, query) ||
                    EF.Functions.TrigramsAreSimilar(x.Content, query) ||
                    EF.Property<NpgsqlTsVector>(x, "SearchVector")
                        .Matches(EF.Functions.PlainToTsQuery("english", query))
                ))
            .Select(x => new SearchResultDto
            {
                EntityId = x.EntityId,
                EntityType = x.EntityType.ToString(),
                ProjectId = x.ProjectId,
                Title = x.Title,
                Content = x.Content,
                Score = Math.Max(
                            EF.Functions.TrigramsSimilarity(x.Title, query),
                            EF.Functions.TrigramsSimilarity(x.Content, query)) +
                        EF.Property<NpgsqlTsVector>(x, "SearchVector")
                            .Rank(EF.Functions.PlainToTsQuery("english", query))
            })
            .OrderByDescending(x => x.Score)
            .Take(50)
            .ToListAsync(cancellationToken);

        var attachmentResults = await context.DocumentChunks
            .AsNoTracking()
            .Where(x =>
                !x.Attachment.IsDeleted &&
                x.Attachment.Issue.ProjectId == projectId &&
                EF.Property<NpgsqlTsVector>(x, "SearchVector")
                    .Matches(EF.Functions.PlainToTsQuery("english", query)))
            .Select(x => new SearchResultDto
            {
                EntityId = x.Attachment.IssueId,
                EntityType = SearchEntityType.Issue.ToString(),
                ProjectId = x.Attachment.Issue.ProjectId,
                Title = x.Attachment.Issue.Title,
                Content = x.Content,
                Score = EF.Property<NpgsqlTsVector>(x, "SearchVector")
                    .Rank(EF.Functions.PlainToTsQuery("english", query))
            })
            .OrderByDescending(x => x.Score)
            .Take(50)
            .ToListAsync(cancellationToken);

        return [.. indexedResults
            .Concat(attachmentResults)
            .GroupBy(x => new { x.EntityType, x.EntityId })
            .Select(x => x.OrderByDescending(result => result.Score).First())
            .OrderByDescending(x => x.Score)
            .Take(50)];
    }
}
