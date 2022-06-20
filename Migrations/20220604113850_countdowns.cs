using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinderNET.Migrations
{
    /// <inheritdoc />
    public partial class countdowns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "countdowns",
                columns: table => new
                {
                    messageId = table.Column<long>(type: "bigint", nullable: false),
                    channelId = table.Column<long>(type: "bigint", nullable: false),
                    guildId = table.Column<long>(type: "bigint", nullable: false),
                    dateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_countdowns", x => new { x.messageId, x.channelId, x.guildId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "countdowns");
        }
    }
}
