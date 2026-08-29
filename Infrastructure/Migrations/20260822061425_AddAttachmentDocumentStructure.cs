using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    [Migration("20260822061425_AddAttachmentDocumentStructure")]
    public partial class AddAttachmentDocumentStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentStructureJson",
                schema: "app",
                table: "IssueAttachments",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentStructureJson",
                schema: "app",
                table: "IssueAttachments");
        }
    }
}
