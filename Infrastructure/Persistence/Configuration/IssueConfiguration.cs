using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class IssueConfiguration : IEntityTypeConfiguration<Issue>
{
    public void Configure(EntityTypeBuilder<Issue> builder)
    {
        builder.ToTable("Issues");

        builder.HasKey(i => i.Id);

        // ---------------- Core ----------------
        builder.Property(i => i.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(i => i.Description)
            .HasMaxLength(2000);

        builder.Property(i => i.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(IssueStatus.ToDo);

        builder.Property(i => i.Priority)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(IssuePriority.Medium);

        // ---------------- Audit ----------------
        builder.Property(i => i.CreatedAt).IsRequired();
        builder.Property(i => i.UpdatedAt).IsRequired();

        // ---------------- Concurrency ----------------
        builder.Property(i => i.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        // ---------------- Relationships ----------------

        builder.HasOne(i => i.Project)
            .WithMany(p => p.Issues)
            .HasForeignKey(i => i.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Sprint)
            .WithMany(s => s.Issues)
            .HasForeignKey(i => i.SprintId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(i => i.Assignee)
            .WithMany()
            .HasForeignKey(i => i.AssigneeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsOne(i => i.TimeTracking, tt =>
        {
            tt.Property(x => x.OriginalEstimateMinutes);
            tt.Property(x => x.TimeSpentMinutes);
            tt.Property(x => x.TimeRemainingMinutes);
        });

        builder.HasOne(i => i.ParentIssue)
            .WithMany(i => i.SubIssues)
            .HasForeignKey(i => i.ParentIssueId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.Labels)
            .WithOne(l => l.Issue)
            .HasForeignKey(l => l.IssueId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---------------- Indexes ----------------
        builder.HasIndex(i => i.ProjectId);
        builder.HasIndex(i => i.SprintId);
        builder.HasIndex(i => i.Status);
        builder.HasIndex(i => i.Priority);
    }
}