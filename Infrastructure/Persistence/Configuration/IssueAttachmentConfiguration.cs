using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectPlanner.Infrastructure.Persistence.Configuration;

public class IssueAttachmentConfiguration : IEntityTypeConfiguration<IssueAttachment>
{
    public void Configure(EntityTypeBuilder<IssueAttachment> builder)
    {
        builder.ToTable("IssueAttachments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedOnAdd();

        builder.Property(a => a.OriginalFileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(a => a.StoredFileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(a => a.StorageKey)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(a => a.ContentType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.FileSize)
            .IsRequired();

        builder.Property(a => a.Sha256Hash)
            .HasMaxLength(64);

        builder.Property(a => a.UploadedAt)
            .IsRequired();

        builder.Property(a => a.IsDeleted)
            .HasDefaultValue(false);

        builder.Property<uint>("xmin").IsRowVersion();

        builder.HasOne(a => a.Issue)
            .WithMany(i => i.Attachments)
            .HasForeignKey(a => a.IssueId)
            .OnDelete(DeleteBehavior.Cascade);

        //builder.HasOne(a => a.UploadedBy)
        //    .WithMany()
        //    .HasForeignKey(a => a.UploadedByUserId)
        //    .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.UploadedBy)
            .WithMany()
            .HasForeignKey(a => a.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasOne(a => a.DeletedBy)
            .WithMany()
            .HasForeignKey(a => a.DeletedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => a.IssueId);

        builder.HasIndex(a => a.UploadedByUserId);

        builder.HasIndex(a => a.UploadedAt);

        builder.HasIndex(a => new
        {
            a.IssueId,
            a.IsDeleted
        });
    }
}