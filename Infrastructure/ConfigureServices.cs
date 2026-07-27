using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OpenAI;
using ProjectPlanner.Application.Common.Interfaces.Persistence;
using ProjectPlanner.Application.Common.Interfaces.Security;
using ProjectPlanner.Application.Services;
using ProjectPlanner.Application.Services.Implementations;
using ProjectPlanner.Infrastructure.Persistence;
using ProjectPlanner.Infrastructure.Persistence.Repositories;
using ProjectPlanner.Infrastructure.Security;
using System.ClientModel;
using System.Text;

namespace ProjectPlanner.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddProjectPlannerDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddDbContext<ProjectPlannerDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("ProjectPlanner.Infrastructure")));

        return services;
    }

    public static IServiceCollection AddDataRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IProjectMemberRepository, ProjectMemberRepository>();
        services.AddScoped<IEpicRepository, EpicRepository>();
        services.AddScoped<ISprintRepository, SprintRepository>();
        services.AddScoped<IIssueRepository, IssueRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IHistoryRepository, HistoryRepository>();
        services.AddScoped<IDevelopmentRepository, DevelopmentRepository>();
        services.AddScoped<IWorkLogRepository, WorkLogRepository>();
        services.AddScoped<ISearchQueryRepository, SearchQueryRepository>();
        services.AddScoped<ISearchIndexRepository, SearchIndexRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    public static IServiceCollection AddSecurityServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddScoped<IUserContext, UserContext>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["JwtSettings:Issuer"],
                ValidAudience = configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["JwtSettings:Secret"]
                        ?? throw new InvalidOperationException("JWT Secret is missing from configuration.")))
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"Auth Error: {context.Exception.Message}");
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddAiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IChatClient>(sp =>
        {
            var client = new OpenAIClient(
                credential: new ApiKeyCredential(configuration["AiGateway:MasterKey"]!),
                options: new OpenAIClientOptions
                {
                    Endpoint = new Uri(configuration["AiGateway:BaseUrl"]!)
                });

            return client
                .GetChatClient("planner-smart-model")
                .AsIChatClient();
        });
        return services;
    }
}