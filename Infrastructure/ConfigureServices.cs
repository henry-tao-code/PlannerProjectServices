using Confluent.Kafka;
using Contracts.Events;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using OpenAI;
using ProjectPlanner.Application.Common.Interfaces.AI;
using ProjectPlanner.Application.Common.Interfaces.Messaging;
using ProjectPlanner.Application.Common.Interfaces.Persistence;
using ProjectPlanner.Application.Common.Interfaces.Security;
using ProjectPlanner.Application.Common.Interfaces.Storage;
using ProjectPlanner.Application.Services;
using ProjectPlanner.Application.Services.Implementations;
using ProjectPlanner.Infrastructure.AI;
using ProjectPlanner.Infrastructure.AI.Configurations;
using ProjectPlanner.Infrastructure.Messaging;
using ProjectPlanner.Infrastructure.Persistence;
using ProjectPlanner.Infrastructure.Persistence.Repositories;
using ProjectPlanner.Infrastructure.Security;
using ProjectPlanner.Infrastructure.Storage;
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

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing from configuration.");

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.UseVector();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContextFactory<ProjectPlannerDbContext>(options =>
            options.UseNpgsql(
                dataSource,
                b =>
                {
                    b.UseVector();
                    b.MigrationsAssembly("ProjectPlanner.Infrastructure");
                }));

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
        services.AddScoped<IIssueAttachmentRepository, IssueAttachmentRepository>();
        services.AddScoped<IHistoryRepository, HistoryRepository>();
        services.AddScoped<IDevelopmentRepository, DevelopmentRepository>();
        services.AddScoped<IWorkLogRepository, WorkLogRepository>();
        services.AddScoped<ISearchQueryRepository, SearchQueryRepository>();
        services.AddScoped<ISearchIndexRepository, SearchIndexRepository>();
        services.AddScoped<IDocumentChunkRepository, DocumentChunkRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

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
        services.Configure<LiteLlmOptions>(configuration.GetSection("LiteLLM"));

        services.AddHttpClient<ILlmService, LiteLlmService>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<LiteLlmOptions>>().Value;
            if (!string.IsNullOrWhiteSpace(options.BaseUrl))
            {
                client.BaseAddress = new Uri(options.BaseUrl);
            }
        });

        services.AddScoped<IVectorSearchService, VectorSearchService>();
        services.AddScoped<IKeywordSearchService, KeywordSearchService>();

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

    public static IServiceCollection AddMessagingServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var teiUrl = configuration["Tei:BaseUrl"] ?? "http://localhost:8080";
        services.AddHttpClient<ITEIEmbeddingService, TeiEmbeddingService>(client =>
        {
            client.BaseAddress = new Uri(teiUrl.EndsWith('/') ? teiUrl : $"{teiUrl}/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHostedService<DocumentParsedConsumerService>();

        services.Configure<KafkaOptions>(configuration.GetSection("Kafka"));

        services.AddSingleton<IProducer<string, string>>(sp =>
        {
            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"]
            };

            return new ProducerBuilder<string, string>(config).Build();
        });

        services.AddScoped<IKafkaProducer, KafkaProducer>();

        services.AddSingleton<IConsumer<string, DocumentProcessedEvent>>(sp =>
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                GroupId = "document-processed-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            return new ConsumerBuilder<string, DocumentProcessedEvent>(config)
                .SetValueDeserializer(new KafkaJsonDeserializer<DocumentProcessedEvent>())
                .Build();
        });

        return services;
    }
}