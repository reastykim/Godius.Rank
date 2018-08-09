using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Godius.Data.Migrations
{
    public partial class AddWeeklyRankTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeeklyRank",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CharacterId = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: true),
                    Ranking = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyRank", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeeklyRank_Character_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyRank_CharacterId",
                table: "WeeklyRank",
                column: "CharacterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeeklyRank");
        }
    }
}
