using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;
using ProjectPlanner.Infrastructure.Persistence;

#nullable disable

namespace ProjectPlanner.Infrastructure.Migrations;

[DbContext(typeof(ProjectPlannerDbContext))]
[Migration("20260806100000_AddDocumentChunkFullTextSearch")]
public partial class AddDocumentChunkFullTextSearch : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<NpgsqlTsVector>(
            name: "SearchVector",
            schema: "app",
            table: "DocumentChunks",
            type: "tsvector",
            nullable: true,
            computedColumnSql: "to_tsvector('english', coalesce(\"Content\", ''))",
            stored: true);

        migrationBuilder.CreateIndex(
            name: "IX_DocumentChunks_SearchVector",
            schema: "app",
            table: "DocumentChunks",
            column: "SearchVector")
            .Annotation("Npgsql:IndexMethod", "GIN");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_DocumentChunks_SearchVector",
            schema: "app",
            table: "DocumentChunks");

        migrationBuilder.DropColumn(
            name: "SearchVector",
            schema: "app",
            table: "DocumentChunks");
    }
}
