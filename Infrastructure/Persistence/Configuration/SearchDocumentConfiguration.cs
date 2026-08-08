using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NpgsqlTypes;

namespace ProjectPlanner.Infrastructure.Persistence.Configuration;

public class SearchDocumentConfiguration : IEntityTypeConfiguration<SearchDocument>
{
    public void Configure(EntityTypeBuilder<SearchDocument> builder)
    {
        builder.ToTable("SearchDocuments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EntityType)
            .HasConversion<int>();

        builder.HasIndex(x => new
        {
            x.EntityType,
            x.EntityId
        }).IsUnique();

        builder.HasIndex(x => x.ProjectId);

        builder.HasIndex(x => x.Title)
            .HasMethod("GIN")
            .HasOperators("gin_trgm_ops");

        builder.HasIndex(x => x.Content)
            .HasMethod("GIN")
            .HasOperators("gin_trgm_ops");

        builder.Property<NpgsqlTsVector>("SearchVector")
            .HasComputedColumnSql(
                """
                to_tsvector(
                    'english',
                    coalesce("Title",'') || ' ' ||
                    coalesce("Content",'')
                )
                """,
                stored: true);

        builder.HasIndex("SearchVector")
            .HasMethod("GIN");
    }
}
