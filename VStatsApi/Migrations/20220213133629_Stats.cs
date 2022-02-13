using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VStatsApi.Migrations
{
    public partial class Stats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CharacterCount",
                table: "Stats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FileCount",
                table: "Stats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LinesOfCode",
                table: "Stats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProblemCount",
                table: "Stats",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "Stats",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CharacterCount",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "FileCount",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "LinesOfCode",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "ProblemCount",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Stats");
        }
    }
}
