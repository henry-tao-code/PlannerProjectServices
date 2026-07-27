using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectPlanner.Infrastructure.Persistence.Configuration;

public class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
{
    public void Configure(EntityTypeBuilder<ProjectMember> builder)
    {
        builder.ToTable("ProjectMembers");

        builder.HasKey(pm => new { pm.ProjectId, pm.UserId });

        builder.HasQueryFilter(pm => !pm.IsDeleted);

        builder.Property(pm => pm.Role)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(ProjectRole.Viewer);

        builder.HasOne(pm => pm.Project)
            .WithMany(p => p.ProjectMembers)
            .HasForeignKey(pm => pm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pm => pm.User)
            .WithMany(u => u.ProjectMemberships)
            .HasForeignKey(pm => pm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(pm => pm.JoinedAt)
            .IsRequired();

        builder.Property(pm => pm.UpdatedAt);

        builder.Property(pm => pm.AddedByUserId);
        builder.Property(pm => pm.UpdatedByUserId);
    }
}