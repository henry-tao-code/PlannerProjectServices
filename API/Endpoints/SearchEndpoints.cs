using Microsoft.AspNetCore.Mvc;
using ProjectPlanner.Application.Common.Dtos.Search;
using ProjectPlanner.Application.Services;

namespace ProjectPlanner.API.Endpoints;

public static class SearchEndpoints
{
    public static IEndpointRouteBuilder MapSearchEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/search")
                       .WithTags("Search");

        group.MapGet("/", async (
            [AsParameters] SearchRequestDto request,
            [FromServices] ISearchService searchService,
            CancellationToken cancellationToken) =>
        {
            var response = await searchService.SearchAsync(request, cancellationToken);
            return TypedResults.Ok(response);
        })
        .WithName("SearchDocuments")
        .Produces<PagedSearchResponseDto<SearchResultDto>>(StatusCodes.Status200OK)
        .WithDescription("Performs Elasticsearch search across issues and epics with optional filtering.");

        return app;
    }
}
