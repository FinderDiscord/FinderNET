using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinderNET.Migrations
{
    /// <inheritdoc />
    public partial class added_economy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "economy",
                columns: table => new
                {
                    guildId = table.Column<long>(type: "bigint", nullable: false),
                    userId = table.Column<long>(type: "bigint", nullable: false),
                    money = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_economy", x => new { x.guildId, x.userId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "economy");
        }
    }
}
