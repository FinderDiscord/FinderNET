using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinderNET.Migrations
{
    /// <inheritdoc />
    public partial class leveling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "leveling",
                columns: table => new
                {
                    guildId = table.Column<long>(type: "bigint", nullable: false),
                    userId = table.Column<long>(type: "bigint", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    exp = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leveling", x => new { x.guildId, x.userId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "leveling");
        }
    }
}
