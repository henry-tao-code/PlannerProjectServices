using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectPlanner.Infrastructure.Persistence.Configuration;

public class SprintConfiguration : IEntityTypeConfiguration<Sprint>
{
    public void Configure(EntityTypeBuilder<Sprint> builder)
    {
        builder.ToTable("Sprints");

        // ---------------- Key ----------------
        builder.HasKey(s => s.Id);

        // ---------------- Core fields ----------------
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Goal)
            .HasMaxLength(500);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(s => s.StartDate);
        builder.Property(s => s.EndDate);
        builder.Property(s => s.CompletedDate);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .IsRequired();

        builder.Property(s => s.CreatedBy);

        builder.Property(s => s.CompletedBy);

        builder.Property(s => s.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasQueryFilter(s => !s.IsDeleted);

        // ---------------- Concurrency ----------------
        builder.Property(s => s.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        // ---------------- Relationships ----------------

        builder.HasOne(s => s.Project)
            .WithMany(p => p.Sprints)
            .HasForeignKey(s => s.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Issues)
            .WithOne(i => i.Sprint)
            .HasForeignKey(i => i.SprintId)
            .OnDelete(DeleteBehavior.SetNull);

        // ---------------- Indexes (important for performance) ----------------
        builder.HasIndex(s => new { s.ProjectId, s.Status });

        builder.HasIndex(s => new { s.ProjectId, s.StartDate });
    }
}