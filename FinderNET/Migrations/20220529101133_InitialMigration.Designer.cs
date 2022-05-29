using System.Collections.Generic;
using FinderNET.Database.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace FinderNET.Migrations {
    [DbContext(typeof(FinderDatabaseContextFactory))]
    [Migration("20220529101133_InitialMigration")]
    partial class InitialMigration {
        protected override void BuildTargetModel(ModelBuilder modelBuilder) {
            #pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.0-preview.4.22229.2").HasAnnotation("Relational:MaxIdentifierLength", 63);
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);
            modelBuilder.Entity("FinderNET.Database.Addons", b => {
                b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("integer");
                NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));
                b.Property<List<string>>("addons").IsRequired().HasColumnType("text[]");
                b.HasKey("Id");
                b.ToTable("addons");
            });
            #pragma warning restore 612, 618
        }
    }
}
