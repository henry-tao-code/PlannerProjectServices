using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectPlanner.Infrastructure.Persistence.Configuration;

public class IssueCommitConfiguration : IEntityTypeConfiguration<IssueCommit>
{
    public void Configure(EntityTypeBuilder<IssueCommit> builder)
    {
        builder.ToTable("IssueCommits");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Sha)
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(x => x.Message)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.Author)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Url)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.CommittedAt)
            .IsRequired();

        builder.HasIndex(x => x.Sha)
            .IsUnique();

        builder.HasOne(x => x.Issue)
            .WithMany(i => i.Commits)
            .HasForeignKey(x => x.IssueId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}