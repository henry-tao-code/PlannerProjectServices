using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectPlanner.Infrastructure.Persistence.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<IssueComment>
{
    public void Configure(EntityTypeBuilder<IssueComment> builder)
    {
        builder.ToTable("Comments");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Body)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.EditedAt)
            .IsRequired();

        // ---------------- Concurrency ----------------
        builder.Property<uint>("xmin").IsRowVersion();

        // ---------------- Relationships ----------------
        builder.HasOne(c => c.Issue)
            .WithMany(i => i.Comments)
            .HasForeignKey(c => c.IssueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Author)
            .WithMany()
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---------------- Indexes ----------------
        builder.HasIndex(c => c.IssueId);
        builder.HasIndex(c => c.AuthorId);
        builder.HasIndex(c => new { c.IssueId, c.CreatedAt });
    }
}