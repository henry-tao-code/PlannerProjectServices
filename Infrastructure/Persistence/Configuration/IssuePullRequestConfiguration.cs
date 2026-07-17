using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectPlanner.Infrastructure.Persistence.Configuration;

public class IssuePullRequestConfiguration
    : IEntityTypeConfiguration<IssuePullRequest>
{
    public void Configure(EntityTypeBuilder<IssuePullRequest> builder)
    {
        builder.ToTable("IssuePullRequests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.GithubPullRequestId)
            .IsRequired();

        builder.Property(x => x.Number)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.Url)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.State)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Merged)
            .IsRequired();

        builder.HasIndex(x => x.GithubPullRequestId)
            .IsUnique();

        builder.HasOne(x => x.Issue)
            .WithMany(i => i.PullRequests)
            .HasForeignKey(x => x.IssueId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}