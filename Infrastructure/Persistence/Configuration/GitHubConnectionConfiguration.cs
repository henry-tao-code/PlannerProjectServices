using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectPlanner.Infrastructure.Persistence.Configuration;

public class GitHubConnectionConfiguration : IEntityTypeConfiguration<GitHubConnection>
{
    public void Configure(EntityTypeBuilder<GitHubConnection> builder)
    {
        builder.ToTable("GitHubConnections");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.GithubUserId)
            .IsRequired();

        builder.Property(x => x.GithubUsername)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.AccessToken)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.ConnectedAt)
            .IsRequired();

        builder.HasIndex(x => x.UserId)
            .IsUnique();

        builder.HasIndex(x => x.GithubUserId)
            .IsUnique();

        builder.HasOne(x => x.User)
            .WithOne(u => u.GitHubConnection)
            .HasForeignKey<GitHubConnection>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}