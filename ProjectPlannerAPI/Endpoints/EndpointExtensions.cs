using ProjectPlanner.API.Endpoints;

namespace ProjectPlanner.Api.Endpoints;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapAllApplicationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapUserEndpoints();
        app.MapProjectEndpoints();
        app.MapProjectMemberEndpoints();
        app.MapEpicEndpoints();
        app.MapSprintEndpoints();
        app.MapIssueEndpoints();
        app.MapDevelopmentEndpoints();
        app.MapCommentEndpoints();
        app.MapWorkLogEndpoints();
        app.MapHistoryEndpoints();

        return app;
    }
}