using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Attachment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IssueAttachment_Issues_IssueId",
                schema: "app",
                table: "IssueAttachment");

            migrationBuilder.DropForeignKey(
                name: "FK_IssueAttachment_Users_UploadedById",
                schema: "app",
                table: "IssueAttachment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IssueAttachment",
                schema: "app",
                table: "IssueAttachment");

            migrationBuilder.DropIndex(
                name: "IX_IssueAttachment_UploadedById",
                schema: "app",
                table: "IssueAttachment");

            migrationBuilder.DropColumn(
                name: "FileName",
                schema: "app",
                table: "IssueAttachment");

            migrationBuilder.DropColumn(
                name: "FilePath",
                schema: "app",
                table: "IssueAttachment");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "app",
                table: "IssueAttachment");

            migrationBuilder.DropColumn(
                name: "UploadedById",
                schema: "app",
                table: "IssueAttachment");

            migrationBuilder.RenameTable(
                name: "IssueAttachment",
                schema: "app",
                newName: "IssueAttachments",
                newSchema: "app");

            migrationBuilder.RenameColumn(
                name: "SizeBytes",
                schema: "app",
                table: "IssueAttachments",
                newName: "FileSize");

            migrationBuilder.RenameIndex(
                name: "IX_IssueAttachment_IssueId",
                schema: "app",
                table: "IssueAttachments",
                newName: "IX_IssueAttachments_IssueId");

            migrationBuilder.AlterColumn<string>(
                name: "StoredFileName",
                schema: "app",
                table: "IssueAttachments",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "app",
                table: "IssueAttachments",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                schema: "app",
                table: "IssueAttachments",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "OriginalFileName",
                schema: "app",
                table: "IssueAttachments",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sha256Hash",
                schema: "app",
                table: "IssueAttachments",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StorageKey",
                schema: "app",
                table: "IssueAttachments",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "app",
                table: "IssueAttachments",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddPrimaryKey(
                name: "PK_IssueAttachments",
                schema: "app",
                table: "IssueAttachments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_IssueAttachments_DeletedByUserId",
                schema: "app",
                table: "IssueAttachments",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueAttachments_IssueId_IsDeleted",
                schema: "app",
                table: "IssueAttachments",
                columns: new[] { "IssueId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_IssueAttachments_UploadedAt",
                schema: "app",
                table: "IssueAttachments",
                column: "UploadedAt");

            migrationBuilder.CreateIndex(
                name: "IX_IssueAttachments_UploadedByUserId",
                schema: "app",
                table: "IssueAttachments",
                column: "UploadedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_IssueAttachments_Issues_IssueId",
                schema: "app",
                table: "IssueAttachments",
                column: "IssueId",
                principalSchema: "app",
                principalTable: "Issues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IssueAttachments_Users_DeletedByUserId",
                schema: "app",
                table: "IssueAttachments",
                column: "DeletedByUserId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IssueAttachments_Users_UploadedByUserId",
                schema: "app",
                table: "IssueAttachments",
                column: "UploadedByUserId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IssueAttachments_Issues_IssueId",
                schema: "app",
                table: "IssueAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_IssueAttachments_Users_DeletedByUserId",
                schema: "app",
                table: "IssueAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_IssueAttachments_Users_UploadedByUserId",
                schema: "app",
                table: "IssueAttachments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IssueAttachments",
                schema: "app",
                table: "IssueAttachments");

            migrationBuilder.DropIndex(
                name: "IX_IssueAttachments_DeletedByUserId",
                schema: "app",
                table: "IssueAttachments");

            migrationBuilder.DropIndex(
                name: "IX_IssueAttachments_IssueId_IsDeleted",
                schema: "app",
                table: "IssueAttachments");

            migrationBuilder.DropIndex(
                name: "IX_IssueAttachments_UploadedAt",
                schema: "app",
                table: "IssueAttachments");

            migrationBuilder.DropIndex(
                name: "IX_IssueAttachments_UploadedByUserId",
                schema: "app",
                table: "IssueAttachments");

            migrationBuilder.DropColumn(
                name: "OriginalFileName",
                schema: "app",
                table: "IssueAttachments");

            migrationBuilder.DropColumn(
                name: "Sha256Hash",
                schema: "app",
                table: "IssueAttachments");

            migrationBuilder.DropColumn(
                name: "StorageKey",
                schema: "app",
                table: "IssueAttachments");

            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "app",
                table: "IssueAttachments");

            migrationBuilder.RenameTable(
                name: "IssueAttachments",
                schema: "app",
                newName: "IssueAttachment",
                newSchema: "app");

            migrationBuilder.RenameColumn(
                name: "FileSize",
                schema: "app",
                table: "IssueAttachment",
                newName: "SizeBytes");

            migrationBuilder.RenameIndex(
                name: "IX_IssueAttachments_IssueId",
                schema: "app",
                table: "IssueAttachment",
                newName: "IX_IssueAttachment_IssueId");

            migrationBuilder.AlterColumn<string>(
                name: "StoredFileName",
                schema: "app",
                table: "IssueAttachment",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "app",
                table: "IssueAttachment",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                schema: "app",
                table: "IssueAttachment",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                schema: "app",
                table: "IssueAttachment",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                schema: "app",
                table: "IssueAttachment",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "RowVersion",
                schema: "app",
                table: "IssueAttachment",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "UploadedById",
                schema: "app",
                table: "IssueAttachment",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_IssueAttachment",
                schema: "app",
                table: "IssueAttachment",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_IssueAttachment_UploadedById",
                schema: "app",
                table: "IssueAttachment",
                column: "UploadedById");

            migrationBuilder.AddForeignKey(
                name: "FK_IssueAttachment_Issues_IssueId",
                schema: "app",
                table: "IssueAttachment",
                column: "IssueId",
                principalSchema: "app",
                principalTable: "Issues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IssueAttachment_Users_UploadedById",
                schema: "app",
                table: "IssueAttachment",
                column: "UploadedById",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
