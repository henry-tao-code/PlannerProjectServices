using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProjectPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChunkRepositoryAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentChunks",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AttachmentId = table.Column<long>(type: "bigint", nullable: false),
                    ChunkIndex = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: false),
                    Embedding = table.Column<float[]>(type: "real[]", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentChunks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentChunks_IssueAttachments_AttachmentId",
                        column: x => x.AttachmentId,
                        principalSchema: "app",
                        principalTable: "IssueAttachments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentChunks_AttachmentId_ChunkIndex",
                schema: "app",
                table: "DocumentChunks",
                columns: new[] { "AttachmentId", "ChunkIndex" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentChunks",
                schema: "app");
        }
    }
}
