﻿// <auto-generated />
using System;
using Godius.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Godius.Data.Migrations
{
    [DbContext(typeof(RankContext))]
    [Migration("20180803025546_AddWeeklyRankTable")]
    partial class AddWeeklyRankTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Godius.Data.Models.Character", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("GuildId");

                    b.Property<int?>("GuildPosition");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("GuildId");

                    b.ToTable("Character");
                });

            modelBuilder.Entity("Godius.Data.Models.Guild", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Image");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Guild");
                });

            modelBuilder.Entity("Godius.Data.Models.Rank", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CharacterId");

                    b.Property<DateTime?>("Date");

                    b.Property<int>("Ranking");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId");

                    b.ToTable("Rank");
                });

            modelBuilder.Entity("Godius.Data.Models.WeeklyRank", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CharacterId");

                    b.Property<DateTime?>("Date");

                    b.Property<int>("Ranking");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId");

                    b.ToTable("WeeklyRank");
                });

            modelBuilder.Entity("Godius.Data.Models.Character", b =>
                {
                    b.HasOne("Godius.Data.Models.Guild", "Guild")
                        .WithMany("Characters")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Godius.Data.Models.Rank", b =>
                {
                    b.HasOne("Godius.Data.Models.Character", "Character")
                        .WithMany("Ranks")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Godius.Data.Models.WeeklyRank", b =>
                {
                    b.HasOne("Godius.Data.Models.Character", "Character")
                        .WithMany("WeeklyRanks")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
