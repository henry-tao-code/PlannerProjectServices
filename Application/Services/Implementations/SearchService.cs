using Microsoft.Extensions.Logging;
using ProjectPlanner.Application.Common.Dtos.Search;
using ProjectPlanner.Application.Common.Interfaces.Persistence;

namespace ProjectPlanner.Application.Services.Implementations;

public class SearchService(
        ISearchIndexRepository searchIndexRepository,
        ILogger<SearchService> logger
    ) : ISearchService
{
    public async Task<PagedSearchResponseDto<SearchResultDto>> SearchAsync(
        SearchRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return new PagedSearchResponseDto<SearchResultDto>(
                [],
                request.Page,
                request.PageSize);
        }

        var sanitizedQuery = request.Query.Trim();
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var rawDocuments = await searchIndexRepository.SearchAsync(
            sanitizedQuery,
            request.ProjectId,
            request.Status,
            page,
            pageSize,
            cancellationToken);

        var results = rawDocuments.Select(doc => new SearchResultDto(
            doc.EntityId,
            doc.EntityType,
            doc.IssueKey,
            doc.Title,
            doc.Description,
            doc.ProjectId,
            doc.ProjectName,
            doc.Status,
            doc.Priority,
            doc.AssigneeName,
            null
        )).ToList();

        logger.LogInformation(
            "Search executed for query '{Query}'. Page {Page}, returned {Count} results.",
            sanitizedQuery, page, results.Count);

        return new PagedSearchResponseDto<SearchResultDto>(results, page, pageSize);
    }
}
