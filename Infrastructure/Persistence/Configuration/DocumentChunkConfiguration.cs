using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NpgsqlTypes;

namespace ProjectPlanner.Infrastructure.Persistence.Configuration;

public class DocumentChunkConfiguration
    : IEntityTypeConfiguration<DocumentChunk>
{
    public void Configure(
        EntityTypeBuilder<DocumentChunk> builder)
    {
        builder.ToTable("DocumentChunks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content)
            .HasMaxLength(8000)
            .IsRequired();

        builder.Property(x => x.ChunkType)
            .HasMaxLength(32)
            .HasDefaultValue("content")
            .IsRequired();

        builder.Property<NpgsqlTsVector>("SearchVector")
            .HasComputedColumnSql(
                "to_tsvector('english', coalesce(\"Content\", ''))",
                stored: true);

        builder.HasIndex("SearchVector")
            .HasMethod("GIN");

        builder.HasIndex(x => new
        {
            x.AttachmentId,
            x.ChunkIndex
        });

        builder.HasOne(x => x.Attachment)
            .WithMany()
            .HasForeignKey(x => x.AttachmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
