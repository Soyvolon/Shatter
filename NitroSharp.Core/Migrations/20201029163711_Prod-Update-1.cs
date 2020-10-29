using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitroSharp.Core.Migrations
{
    public partial class ProdUpdate1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configs",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(nullable: false),
                    Prefix = table.Column<string>(nullable: false),
                    Culture = table.Column<string>(nullable: false),
                    AllowPublicTriviaGames = table.Column<bool>(nullable: false),
                    TriviaQuestionLimit = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configs", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "Filters",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(nullable: false),
                    Filters = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Filters", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "Memberlogs",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(nullable: false),
                    JoinDmMessage = table.Column<string>(nullable: true),
                    MemberlogChannel = table.Column<decimal>(nullable: true),
                    JoinMessage_Message = table.Column<string>(nullable: true),
                    JoinMessage_IsEmbed = table.Column<bool>(nullable: true),
                    JoinMessage_IsImage = table.Column<bool>(nullable: true),
                    JoinMessage_ImageUrl = table.Column<string>(nullable: true),
                    LeaveMessage_Message = table.Column<string>(nullable: true),
                    LeaveMessage_IsEmbed = table.Column<bool>(nullable: true),
                    LeaveMessage_IsImage = table.Column<bool>(nullable: true),
                    LeaveMessage_ImageUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memberlogs", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "Moderations",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(nullable: false),
                    UserBans = table.Column<string>(nullable: false),
                    SlowmodeLocks = table.Column<string>(nullable: false),
                    UserMutes = table.Column<string>(nullable: false),
                    ModLogChannel = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moderations", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "TriviaPlayers",
                columns: table => new
                {
                    UserId = table.Column<decimal>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    Points = table.Column<int>(nullable: false),
                    QuestionsCorrect = table.Column<int>(nullable: false),
                    QuestionsIncorrect = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriviaPlayers", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    UserId = table.Column<decimal>(nullable: false),
                    Balance = table.Column<int>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    LastDaily = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.UserId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configs");

            migrationBuilder.DropTable(
                name: "Filters");

            migrationBuilder.DropTable(
                name: "Memberlogs");

            migrationBuilder.DropTable(
                name: "Moderations");

            migrationBuilder.DropTable(
                name: "TriviaPlayers");

            migrationBuilder.DropTable(
                name: "Wallets");
        }
    }
}
