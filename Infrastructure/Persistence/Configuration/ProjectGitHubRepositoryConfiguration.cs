using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectPlanner.Infrastructure.Persistence.Configuration;

public class ProjectGitHubRepositoryConfiguration
    : IEntityTypeConfiguration<ProjectGitHubRepository>
{
    public void Configure(EntityTypeBuilder<ProjectGitHubRepository> builder)
    {
        builder.ToTable("ProjectGitHubRepositories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Owner)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Repository)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.DefaultBranch)
            .HasMaxLength(100)
            .HasDefaultValue("main");

        builder.Property(x => x.GithubRepositoryId)
            .IsRequired();

        builder.Property(x => x.ConnectedAt)
            .IsRequired();

        builder.HasIndex(x => x.ProjectId)
            .IsUnique();

        builder.HasOne(x => x.Project)
            .WithOne(p => p.GitHubRepository)
            .HasForeignKey<ProjectGitHubRepository>(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}