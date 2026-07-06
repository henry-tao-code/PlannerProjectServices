using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectPlanner.Infrastructure.Persistence.Configuration;

public class EpicConfiguration : IEntityTypeConfiguration<Epic>
{
    public void Configure(EntityTypeBuilder<Epic> builder)
    {
        builder.ToTable("Epics");

        builder.HasKey(e => e.Id);

        // Global query filter
        builder.HasQueryFilter(e => !e.IsDeleted);

        // Properties
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Summary)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.Description)
            .HasMaxLength(4000);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(EpicStatus.ToDo);

        builder.Property(e => e.StartDate);

        builder.Property(e => e.DueDate);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        builder.HasOne(e => e.Project)
            .WithMany(p => p.Epics)
            .HasForeignKey(e => e.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Issues)
            .WithOne(i => i.Epic)
            .HasForeignKey(i => i.EpicId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => e.ProjectId);

        builder.HasIndex(e => new { e.ProjectId, e.Status });

        builder.HasIndex(e => e.Name);
    }
}