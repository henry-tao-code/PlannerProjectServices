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
            .HasConversion(
                r => r.ToString(),
                r => Enum.Parse<ProjectRole>(r))

            .HasConversion(
                    r => r.ToString(),
                    r => Enum.Parse<ProjectRole>(r) 
                )
            .HasMaxLength(20)
            .HasDefaultValue(ProjectRole.Viewer);

        builder.HasOne(pm => pm.Project)
            .WithMany(p => p.ProjectMembers)
            .HasForeignKey(pm => pm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
    }
}
