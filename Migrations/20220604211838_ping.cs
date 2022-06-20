using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinderNET.Migrations
{
    /// <inheritdoc />
    public partial class ping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "pingRoleId",
                table: "countdowns",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "pingUserId",
                table: "countdowns",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pingRoleId",
                table: "countdowns");

            migrationBuilder.DropColumn(
                name: "pingUserId",
                table: "countdowns");
        }
    }
}
