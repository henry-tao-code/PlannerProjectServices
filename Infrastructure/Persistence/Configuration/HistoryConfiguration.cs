using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectPlanner.Infrastructure.Persistence.Configurations;

public class IssueHistoryConfiguration : IEntityTypeConfiguration<IssueHistory>
{
    public void Configure(EntityTypeBuilder<IssueHistory> builder)
    {
        builder.ToTable("IssueHistories");

        builder.HasKey(h => h.Id);

        // ---------------- Change Data ----------------

        builder.Property(h => h.Field)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(h => h.OldValue)
            .HasColumnType("text");

        builder.Property(h => h.NewValue)
            .HasColumnType("text");

        // ---------------- Audit ----------------

        builder.Property(h => h.ChangedAt)
            .IsRequired();

        // ---------------- Relationships ----------------

        builder.HasOne(h => h.Issue)
            .WithMany(i => i.History)
            .HasForeignKey(h => h.IssueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(h => h.ChangedByUser)
            .WithMany()
            .HasForeignKey(h => h.ChangedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---------------- Indexes ----------------

        builder.HasIndex(h => h.IssueId);
        builder.HasIndex(h => h.ChangedByUserId);

        // important for timeline queries
        builder.HasIndex(h => new { h.IssueId, h.ChangedAt });
    }
}