using Microsoft.AspNetCore.Routing;
using ProjectPlanner.API.Endpoints;

namespace ProjectPlanner.Api.Endpoints;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapAllApplicationEndpoints(this IEndpointRouteBuilder app)
    {
        // Grouping all modular sub-routes under a single registration block
        app.MapUserEndpoints();
        app.MapProjectEndpoints();
        app.MapSprintEndpoints();
        app.MapIssueEndpoints();
        app.MapCommentEndpoints();
        app.MapWorkLogEndpoints();
        app.MapHistoryEndpoints();

        return app;
    }
}