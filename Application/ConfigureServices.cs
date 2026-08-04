using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProjectPlanner.Application.Common.Dtos.User.Validator;
using ProjectPlanner.Application.Services;
using ProjectPlanner.Application.Services.Implementations;

namespace ProjectPlanner.Application;

static public class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IProjectMemberService, ProjectMemberService>();
        services.AddScoped<IEpicService, EpicService>();
        services.AddScoped<ISprintService, SprintService>();
        services.AddScoped<IIssueService, IssueService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IIssueAttachmentService, IssueAttachmentService>();
        services.AddScoped<IHistoryService, HistoryService>();
        services.AddScoped<IDevelopmentService, DevelopmentService>();
        services.AddScoped<IWorkLogService, WorkLogService>();
        services.AddScoped<ISearchQueryService, SearchQueryService>();
        services.AddScoped<ISearchIndexService, SearchIndexService>();

        services.AddScoped<IGitHubApiClient, GitHubApiClient>();

        services.AddScoped<IDocumentParsedProcessor, DocumentParsedProcessor>();

        services.AddValidatorsFromAssemblyContaining<CreateUserDtoValidator>();

        return services;
    }
}