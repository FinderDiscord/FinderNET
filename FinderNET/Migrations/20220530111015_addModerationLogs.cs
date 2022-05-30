using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FinderNET.Migrations
{
    /// <inheritdoc />
    public partial class addModerationLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserLogs",
                columns: table => new
                {
                    userId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    bans = table.Column<int>(type: "integer", nullable: false),
                    kicks = table.Column<int>(type: "integer", nullable: false),
                    warns = table.Column<int>(type: "integer", nullable: false),
                    mutes = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogs", x => x.userId);
                });

            migrationBuilder.CreateTable(
                name: "moderationLogs",
                columns: table => new
                {
                    guildId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userLogsuserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_moderationLogs", x => x.guildId);
                    table.ForeignKey(
                        name: "FK_moderationLogs_UserLogs_userLogsuserId",
                        column: x => x.userLogsuserId,
                        principalTable: "UserLogs",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_moderationLogs_userLogsuserId",
                table: "moderationLogs",
                column: "userLogsuserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "moderationLogs");

            migrationBuilder.DropTable(
                name: "UserLogs");
        }
    }
}
