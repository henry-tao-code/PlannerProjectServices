using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectPlanner.Infrastructure.Persistence.Configurations;

public class WorkLogConfiguration : IEntityTypeConfiguration<WorkLog>
{
    public void Configure(EntityTypeBuilder<WorkLog> builder)
    {
        builder.ToTable("WorkLogs", "app");

        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id)
            .ValueGeneratedOnAdd();

        builder.Property(w => w.TimeSpentMinutes)
            .IsRequired();

        builder.Property(w => w.Description)
            .HasColumnType("text");

        builder.Property(w => w.StartedAt)
            .IsRequired();

        builder.Property(w => w.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(w => w.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(w => w.IssueId);
        builder.HasIndex(w => w.UserId);

        builder.HasOne(w => w.Issue)
            .WithMany()
            .HasForeignKey(w => w.IssueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(w => w.User)
            .WithMany()
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}