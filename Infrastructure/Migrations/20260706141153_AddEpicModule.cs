using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProjectPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEpicModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EpicId",
                schema: "app",
                table: "Issues",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Epics",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Summary = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    AssigneeId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Epics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Epics_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "app",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Epics_Users_AssigneeId",
                        column: x => x.AssigneeId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Issues_EpicId",
                schema: "app",
                table: "Issues",
                column: "EpicId");

            migrationBuilder.CreateIndex(
                name: "IX_Epics_AssigneeId",
                schema: "app",
                table: "Epics",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_Epics_Name",
                schema: "app",
                table: "Epics",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Epics_ProjectId",
                schema: "app",
                table: "Epics",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Epics_ProjectId_Status",
                schema: "app",
                table: "Epics",
                columns: ["ProjectId", "Status"]);

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Epics_EpicId",
                schema: "app",
                table: "Issues",
                column: "EpicId",
                principalSchema: "app",
                principalTable: "Epics",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Epics_EpicId",
                schema: "app",
                table: "Issues");

            migrationBuilder.DropTable(
                name: "Epics",
                schema: "app");

            migrationBuilder.DropIndex(
                name: "IX_Issues_EpicId",
                schema: "app",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "EpicId",
                schema: "app",
                table: "Issues");
        }
    }
}
