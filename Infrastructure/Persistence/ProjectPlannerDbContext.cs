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
    public DbSet<Issue> Issues => Set<Issue>();
    public DbSet<Sprint> Sprints => Set<Sprint>();
    public DbSet<IssueComment> Comments => Set<IssueComment>();
    public DbSet<IssueHistory> History => Set<IssueHistory>();
    public DbSet<WorkLog> WorkLogs => Set<WorkLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("app");

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ProjectPlannerDbContext).Assembly
        );

        modelBuilder.Entity<User>()
            .HasQueryFilter(u => !u.IsDeleted);

        modelBuilder.Entity<Project>()
            .HasQueryFilter(p => !p.IsArchived);

        modelBuilder.Entity<Sprint>()
            .HasQueryFilter(s => !s.IsDeleted);
    }
}