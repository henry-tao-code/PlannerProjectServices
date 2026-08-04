using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeAttachmentUploaderOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UploadedByUserId",
                schema: "app",
                table: "IssueAttachments",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UploadedByUserId",
                schema: "app",
                table: "IssueAttachments",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
