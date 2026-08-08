using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ProjectPlanner.Infrastructure.Persistence;

public class ProjectPlannerDbContext(DbContextOptions<ProjectPlannerDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<Epic> Epics => Set<Epic>();
    public DbSet<Sprint> Sprints => Set<Sprint>();
    public DbSet<Issue> Issues => Set<Issue>();
    public DbSet<IssueComment> Comments => Set<IssueComment>();
    public DbSet<IssueHistory> History => Set<IssueHistory>();
    public DbSet<GitHubConnection> GitHubConnections => Set<GitHubConnection>();
    public DbSet<ProjectGitHubRepository> GitHubRepositories => Set<ProjectGitHubRepository>();
    public DbSet<IssueCommit> IssueCommits => Set<IssueCommit>();
    public DbSet<IssuePullRequest> IssuePullRequests => Set<IssuePullRequest>();
    public DbSet<IssueAttachment> IssueAttachments => Set<IssueAttachment>();
    public DbSet<WorkLog> WorkLogs => Set<WorkLog>();
    public DbSet<SearchDocument> SearchDocuments => Set<SearchDocument>();
    public DbSet<DocumentChunk> DocumentChunks => Set<DocumentChunk>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("app");

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ProjectPlannerDbContext).Assembly
        );

        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.Entity<DocumentChunk>(entity =>
        {
            entity.Property(e => e.Embedding)
                  .HasColumnType("vector(512)");

            entity.HasIndex(e => e.Embedding)
                  .HasMethod("hnsw")
                  .HasOperators("vector_cosine_ops");
        });

        modelBuilder.Entity<User>()
            .HasQueryFilter(u => !u.IsDeleted);

        modelBuilder.Entity<Project>()
            .HasQueryFilter(p => !p.IsArchived);

        modelBuilder.Entity<Sprint>()
            .HasQueryFilter(s => !s.IsDeleted);

        modelBuilder.Entity<Issue>()
            .HasQueryFilter(i => !i.IsDeleted);

        modelBuilder.HasPostgresExtension("pg_trgm");
    }
}