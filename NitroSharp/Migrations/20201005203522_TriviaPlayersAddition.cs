using Microsoft.EntityFrameworkCore.Migrations;

namespace NitroSharp.Migrations
{
    public partial class TriviaPlayersAddition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TriviaPlayers",
                columns: table => new
                {
                    UserId = table.Column<decimal>(nullable: false),
                    Points = table.Column<int>(nullable: false),
                    QuestionsCorrect = table.Column<int>(nullable: false),
                    QuestionsIncorrect = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriviaPlayers", x => x.UserId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TriviaPlayers");
        }
    }
}