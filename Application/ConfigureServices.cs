using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProjectPlanner.Application.Common.Dto.User.Validator;
using ProjectPlanner.Application.Common.Interfaces.Services;
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
        services.AddScoped<IDevelopmentService, DevelopmentService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IHistoryService, HistoryService>();
        services.AddScoped<IWorkLogService, WorkLogService>();

        services.AddScoped<IGitHubApiClient, GitHubApiClient>();

        services.AddValidatorsFromAssemblyContaining<CreateUserDtoValidator>();

        return services;
    }
}