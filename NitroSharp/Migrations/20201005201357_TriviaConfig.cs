using Microsoft.EntityFrameworkCore.Migrations;

namespace NitroSharp.Migrations
{
    public partial class TriviaConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowPublicTriviaGames",
                table: "Configs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TriviaQuestionLimit",
                table: "Configs",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowPublicTriviaGames",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "TriviaQuestionLimit",
                table: "Configs");
        }
    }
}
