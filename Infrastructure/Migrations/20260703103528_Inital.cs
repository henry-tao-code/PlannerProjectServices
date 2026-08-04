using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProjectPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Inital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "app");

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    AuthProvider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AuthProviderId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Key = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    LeadId = table.Column<int>(type: "integer", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: false),
                    UpdatedByUserId = table.Column<int>(type: "integer", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Users_LeadId",
                        column: x => x.LeadId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedByIp = table.Column<string>(type: "text", nullable: true),
                    RevokedByIp = table.Column<string>(type: "text", nullable: true),
                    ReplacedByToken = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectMembers",
                schema: "app",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AddedByUserId = table.Column<int>(type: "integer", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMembers", x => new { x.ProjectId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ProjectMembers_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "app",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectMembers_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sprints",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Goal = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CompletedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sprints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sprints_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "app",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Issues",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IssueKey = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IssueType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "ToDo"),
                    Priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Medium"),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    SprintId = table.Column<int>(type: "integer", nullable: true),
                    AssigneeId = table.Column<int>(type: "integer", nullable: true),
                    ReporterId = table.Column<int>(type: "integer", nullable: true),
                    ParentIssueId = table.Column<int>(type: "integer", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    StoryPoints = table.Column<int>(type: "integer", nullable: false),
                    TimeTracking_OriginalEstimateMinutes = table.Column<int>(type: "integer", nullable: true),
                    TimeTracking_TimeSpentMinutes = table.Column<int>(type: "integer", nullable: true),
                    TimeTracking_TimeRemainingMinutes = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Issues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Issues_Issues_ParentIssueId",
                        column: x => x.ParentIssueId,
                        principalSchema: "app",
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Issues_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "app",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Issues_Sprints_SprintId",
                        column: x => x.SprintId,
                        principalSchema: "app",
                        principalTable: "Sprints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Issues_Users_AssigneeId",
                        column: x => x.AssigneeId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Issues_Users_ReporterId",
                        column: x => x.ReporterId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IssueId = table.Column<int>(type: "integer", nullable: false),
                    AuthorId = table.Column<int>(type: "integer", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Issues_IssueId",
                        column: x => x.IssueId,
                        principalSchema: "app",
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IssueAttachment",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IssueId = table.Column<int>(type: "integer", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    StoredFileName = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    UploadedByUserId = table.Column<int>(type: "integer", nullable: false),
                    UploadedById = table.Column<int>(type: "integer", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedByUserId = table.Column<int>(type: "integer", nullable: true),
                    RowVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IssueAttachment_Issues_IssueId",
                        column: x => x.IssueId,
                        principalSchema: "app",
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IssueAttachment_Users_UploadedById",
                        column: x => x.UploadedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IssueHistories",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IssueId = table.Column<int>(type: "integer", nullable: false),
                    ChangedByUserId = table.Column<int>(type: "integer", nullable: false),
                    Field = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: true),
                    NewValue = table.Column<string>(type: "text", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IssueHistories_Issues_IssueId",
                        column: x => x.IssueId,
                        principalSchema: "app",
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IssueHistories_Users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IssueLabel",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IssueId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueLabel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IssueLabel_Issues_IssueId",
                        column: x => x.IssueId,
                        principalSchema: "app",
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IssueLinks",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SourceIssueId = table.Column<int>(type: "integer", nullable: false),
                    TargetIssueId = table.Column<int>(type: "integer", nullable: false),
                    LinkType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IssueLinks_Issues_SourceIssueId",
                        column: x => x.SourceIssueId,
                        principalSchema: "app",
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IssueLinks_Issues_TargetIssueId",
                        column: x => x.TargetIssueId,
                        principalSchema: "app",
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkLogs",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IssueId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TimeSpentMinutes = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkLogs_Issues_IssueId",
                        column: x => x.IssueId,
                        principalSchema: "app",
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkLogs_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AuthorId",
                schema: "app",
                table: "Comments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_IssueId",
                schema: "app",
                table: "Comments",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_IssueId_CreatedAt",
                schema: "app",
                table: "Comments",
                columns: new[] { "IssueId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_IssueAttachment_IssueId",
                schema: "app",
                table: "IssueAttachment",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueAttachment_UploadedById",
                schema: "app",
                table: "IssueAttachment",
                column: "UploadedById");

            migrationBuilder.CreateIndex(
                name: "IX_IssueHistories_ChangedByUserId",
                schema: "app",
                table: "IssueHistories",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueHistories_IssueId",
                schema: "app",
                table: "IssueHistories",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueHistories_IssueId_ChangedAt",
                schema: "app",
                table: "IssueHistories",
                columns: new[] { "IssueId", "ChangedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_IssueLabel_IssueId",
                schema: "app",
                table: "IssueLabel",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueLinks_SourceIssueId",
                schema: "app",
                table: "IssueLinks",
                column: "SourceIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueLinks_TargetIssueId",
                schema: "app",
                table: "IssueLinks",
                column: "TargetIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_AssigneeId",
                schema: "app",
                table: "Issues",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_ParentIssueId",
                schema: "app",
                table: "Issues",
                column: "ParentIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_Priority",
                schema: "app",
                table: "Issues",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_ProjectId",
                schema: "app",
                table: "Issues",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_ReporterId",
                schema: "app",
                table: "Issues",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_SprintId",
                schema: "app",
                table: "Issues",
                column: "SprintId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_Status",
                schema: "app",
                table: "Issues",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMembers_UserId",
                schema: "app",
                table: "ProjectMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Key_Unique",
                schema: "app",
                table: "Projects",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_LeadId",
                schema: "app",
                table: "Projects",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                schema: "app",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Sprints_ProjectId_StartDate",
                schema: "app",
                table: "Sprints",
                columns: new[] { "ProjectId", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Sprints_ProjectId_Status",
                schema: "app",
                table: "Sprints",
                columns: new[] { "ProjectId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_AuthProvider",
                schema: "app",
                table: "Users",
                columns: new[] { "AuthProvider", "AuthProviderId" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "app",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                schema: "app",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_IssueId",
                schema: "app",
                table: "WorkLogs",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_UserId",
                schema: "app",
                table: "WorkLogs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments",
                schema: "app");

            migrationBuilder.DropTable(
                name: "IssueAttachment",
                schema: "app");

            migrationBuilder.DropTable(
                name: "IssueHistories",
                schema: "app");

            migrationBuilder.DropTable(
                name: "IssueLabel",
                schema: "app");

            migrationBuilder.DropTable(
                name: "IssueLinks",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ProjectMembers",
                schema: "app");

            migrationBuilder.DropTable(
                name: "RefreshTokens",
                schema: "app");

            migrationBuilder.DropTable(
                name: "WorkLogs",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Issues",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Sprints",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Projects",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "app");
        }
    }
}
