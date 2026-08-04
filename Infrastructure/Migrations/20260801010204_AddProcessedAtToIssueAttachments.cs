using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessedAtToIssueAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedAt",
                schema: "app",
                table: "IssueAttachments",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessingError",
                schema: "app",
                table: "IssueAttachments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "app",
                table: "IssueAttachments",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessedAt",
                schema: "app",
                table: "IssueAttachments");

            migrationBuilder.DropColumn(
                name: "ProcessingError",
                schema: "app",
                table: "IssueAttachments");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "app",
                table: "IssueAttachments");
        }
    }
}
