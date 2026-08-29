using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ProjectPlanner.Infrastructure.Persistence;

#nullable disable

namespace ProjectPlanner.Infrastructure.Migrations
{
    [DbContext(typeof(ProjectPlannerDbContext))]
    [Migration("20260827090000_AddDocumentChunkMetadata")]
    public partial class AddDocumentChunkMetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChunkType",
                schema: "app",
                table: "DocumentChunks",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "content");

            migrationBuilder.AddColumn<int>(
                name: "PageNumber",
                schema: "app",
                table: "DocumentChunks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TableIndex",
                schema: "app",
                table: "DocumentChunks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TokenCount",
                schema: "app",
                table: "DocumentChunks",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ChunkType", schema: "app", table: "DocumentChunks");
            migrationBuilder.DropColumn(name: "PageNumber", schema: "app", table: "DocumentChunks");
            migrationBuilder.DropColumn(name: "TableIndex", schema: "app", table: "DocumentChunks");
            migrationBuilder.DropColumn(name: "TokenCount", schema: "app", table: "DocumentChunks");
        }
    }
}
