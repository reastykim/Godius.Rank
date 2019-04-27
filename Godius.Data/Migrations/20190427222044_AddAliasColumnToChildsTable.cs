using Microsoft.EntityFrameworkCore.Migrations;

namespace Godius.Data.Migrations
{
    public partial class AddAliasColumnToChildsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Alias",
                table: "Guild",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alias",
                table: "Guild");
        }
    }
}
