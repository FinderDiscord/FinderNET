﻿// <auto-generated />
using System.Collections.Generic;
using FinderNET.Database.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FinderNET.Migrations
{
    [DbContext(typeof(FinderDatabaseContext))]
    partial class FinderDatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-preview.4.22229.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FinderNET.Database.Addons", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<List<string>>("addons")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.HasKey("Id");

                    b.ToTable("addons");
                });

            modelBuilder.Entity("FinderNET.Database.Poll", b =>
                {
                    b.Property<long>("messageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("messageId"));

                    b.Property<List<string>>("answers")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.Property<List<long>>("votersId")
                        .IsRequired()
                        .HasColumnType("bigint[]");

                    b.HasKey("messageId");

                    b.ToTable("polls");
                });

            modelBuilder.Entity("FinderNET.Database.Settings", b =>
                {
                    b.Property<long>("guildId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("guildId"));

                    b.Property<string>("key")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("guildId");

                    b.ToTable("settings");
                });

            modelBuilder.Entity("FinderNET.Database.UserLogs", b =>
                {
                    b.Property<long>("guildId")
                        .HasColumnType("bigint");

                    b.Property<long>("userId")
                        .HasColumnType("bigint");

                    b.Property<int>("bans")
                        .HasColumnType("integer");

                    b.Property<int>("kicks")
                        .HasColumnType("integer");

                    b.Property<int>("mutes")
                        .HasColumnType("integer");

                    b.Property<int>("warns")
                        .HasColumnType("integer");

                    b.HasKey("guildId", "userId");

                    b.ToTable("userLogs");
                });
#pragma warning restore 612, 618
        }
    }
}
