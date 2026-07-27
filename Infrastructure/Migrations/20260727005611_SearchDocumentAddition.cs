using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;

#nullable disable

namespace ProjectPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SearchDocumentAddition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "app",
                table: "Epics",
                newName: "Title");

            migrationBuilder.RenameIndex(
                name: "IX_Epics_Name",
                schema: "app",
                table: "Epics",
                newName: "IX_Epics_Title");

            migrationBuilder.CreateTable(
                name: "SearchDocuments",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityType = table.Column<int>(type: "integer", nullable: false),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: true, computedColumnSql: "to_tsvector(\r\n    'english',\r\n    coalesce(\"Title\",'') || ' ' ||\r\n    coalesce(\"Content\",'')\r\n)", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchDocuments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SearchDocuments_Content",
                schema: "app",
                table: "SearchDocuments",
                column: "Content")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_SearchDocuments_EntityType_EntityId",
                schema: "app",
                table: "SearchDocuments",
                columns: new[] { "EntityType", "EntityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SearchDocuments_ProjectId",
                schema: "app",
                table: "SearchDocuments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchDocuments_SearchVector",
                schema: "app",
                table: "SearchDocuments",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_SearchDocuments_Title",
                schema: "app",
                table: "SearchDocuments",
                column: "Title")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SearchDocuments",
                schema: "app");

            migrationBuilder.RenameColumn(
                name: "Title",
                schema: "app",
                table: "Epics",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_Epics_Title",
                schema: "app",
                table: "Epics",
                newName: "IX_Epics_Name");
        }
    }
}
