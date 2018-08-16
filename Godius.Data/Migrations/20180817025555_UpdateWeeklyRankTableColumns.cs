using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Godius.Data.Migrations
{
    public partial class UpdateWeeklyRankTableColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Character_Guild_GuildId",
                table: "Character");

            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyRank_Character_CharacterId",
                table: "WeeklyRank");

            migrationBuilder.AlterColumn<Guid>(
                name: "CharacterId",
                table: "WeeklyRank",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<Guid>(
                name: "GuildId",
                table: "WeeklyRank",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "GuildId",
                table: "Character",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyRank_GuildId",
                table: "WeeklyRank",
                column: "GuildId");

            migrationBuilder.AddForeignKey(
                name: "FK_Character_Guild_GuildId",
                table: "Character",
                column: "GuildId",
                principalTable: "Guild",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyRank_Character_CharacterId",
                table: "WeeklyRank",
                column: "CharacterId",
                principalTable: "Character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyRank_Guild_GuildId",
                table: "WeeklyRank",
                column: "GuildId",
                principalTable: "Guild",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Character_Guild_GuildId",
                table: "Character");

            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyRank_Character_CharacterId",
                table: "WeeklyRank");

            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyRank_Guild_GuildId",
                table: "WeeklyRank");

            migrationBuilder.DropIndex(
                name: "IX_WeeklyRank_GuildId",
                table: "WeeklyRank");

            migrationBuilder.DropColumn(
                name: "GuildId",
                table: "WeeklyRank");

            migrationBuilder.AlterColumn<Guid>(
                name: "CharacterId",
                table: "WeeklyRank",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "GuildId",
                table: "Character",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Character_Guild_GuildId",
                table: "Character",
                column: "GuildId",
                principalTable: "Guild",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyRank_Character_CharacterId",
                table: "WeeklyRank",
                column: "CharacterId",
                principalTable: "Character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
