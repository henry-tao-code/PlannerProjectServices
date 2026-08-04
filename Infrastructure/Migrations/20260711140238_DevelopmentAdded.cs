using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProjectPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DevelopmentAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GitHubConnections",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    GithubUserId = table.Column<long>(type: "bigint", nullable: false),
                    GithubUsername = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AccessToken = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ConnectedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GitHubConnections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GitHubConnections_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IssueCommits",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IssueId = table.Column<int>(type: "integer", nullable: false),
                    Sha = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Author = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CommittedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueCommits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IssueCommits_Issues_IssueId",
                        column: x => x.IssueId,
                        principalSchema: "app",
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IssuePullRequests",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IssueId = table.Column<int>(type: "integer", nullable: false),
                    GithubPullRequestId = table.Column<long>(type: "bigint", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    State = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Merged = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssuePullRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IssuePullRequests_Issues_IssueId",
                        column: x => x.IssueId,
                        principalSchema: "app",
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectGitHubRepositories",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    Owner = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Repository = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DefaultBranch = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "main"),
                    GithubRepositoryId = table.Column<long>(type: "bigint", nullable: false),
                    ConnectedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectGitHubRepositories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectGitHubRepositories_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "app",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GitHubConnections_GithubUserId",
                schema: "app",
                table: "GitHubConnections",
                column: "GithubUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GitHubConnections_UserId",
                schema: "app",
                table: "GitHubConnections",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IssueCommits_IssueId",
                schema: "app",
                table: "IssueCommits",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueCommits_Sha",
                schema: "app",
                table: "IssueCommits",
                column: "Sha",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IssuePullRequests_GithubPullRequestId",
                schema: "app",
                table: "IssuePullRequests",
                column: "GithubPullRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IssuePullRequests_IssueId",
                schema: "app",
                table: "IssuePullRequests",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectGitHubRepositories_ProjectId",
                schema: "app",
                table: "ProjectGitHubRepositories",
                column: "ProjectId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GitHubConnections",
                schema: "app");

            migrationBuilder.DropTable(
                name: "IssueCommits",
                schema: "app");

            migrationBuilder.DropTable(
                name: "IssuePullRequests",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ProjectGitHubRepositories",
                schema: "app");
        }
    }
}
