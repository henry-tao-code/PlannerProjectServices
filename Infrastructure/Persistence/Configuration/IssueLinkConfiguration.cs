using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class IssueLinkConfiguration : IEntityTypeConfiguration<IssueLink>
{
    public void Configure(EntityTypeBuilder<IssueLink> builder)
    {
        builder.ToTable("IssueLinks");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.SourceIssue)
            .WithMany(i => i.OutgoingLinks)
            .HasForeignKey(x => x.SourceIssueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.TargetIssue)
            .WithMany(i => i.IncomingLinks)
            .HasForeignKey(x => x.TargetIssueId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}