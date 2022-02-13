using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VStatsApi.Migrations
{
    public partial class sth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarURL",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Login",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Stats_UserID",
                table: "Stats",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Stats_Users_UserID",
                table: "Stats",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stats_Users_UserID",
                table: "Stats");

            migrationBuilder.DropIndex(
                name: "IX_Stats_UserID",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "AvatarURL",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Login",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Users");
        }
    }
}
