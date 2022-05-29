using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
#nullable disable
namespace FinderNET.Migrations {
    public partial class ah : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                name: "addons",
                columns: table => new {
                    Id = table.Column<long>(type: "bigint", nullable: false).Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    addons = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_addons", x => x.Id); }
            );
        }
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(name: "addons");
        }
    }
}
