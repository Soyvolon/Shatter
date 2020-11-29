using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace Shatter.Core.Migrations
{
	public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configs",
                columns: table => new
                {
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Prefix = table.Column<string>(type: "TEXT", nullable: false),
                    AllowPublicTriviaGames = table.Column<bool>(type: "INTEGER", nullable: false),
                    TriviaQuestionLimit = table.Column<int>(type: "INTEGER", nullable: false),
                    DisabledModules = table.Column<string>(type: "TEXT", nullable: false),
                    DisabledCommands = table.Column<string>(type: "TEXT", nullable: false),
                    ActivatedCommands = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configs", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "Filters",
                columns: table => new
                {
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Filters = table.Column<string>(type: "TEXT", nullable: false),
                    BypassFilters = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Filters", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "Memberlogs",
                columns: table => new
                {
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    JoinDmMessage = table.Column<string>(type: "TEXT", nullable: true),
                    MemberlogChannel = table.Column<ulong>(type: "INTEGER", nullable: true),
                    JoinMessage_Message = table.Column<string>(type: "TEXT", nullable: true),
                    JoinMessage_IsEmbed = table.Column<bool>(type: "INTEGER", nullable: true),
                    JoinMessage_IsImage = table.Column<bool>(type: "INTEGER", nullable: true),
                    JoinMessage_ImageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    LeaveMessage_Message = table.Column<string>(type: "TEXT", nullable: true),
                    LeaveMessage_IsEmbed = table.Column<bool>(type: "INTEGER", nullable: true),
                    LeaveMessage_IsImage = table.Column<bool>(type: "INTEGER", nullable: true),
                    LeaveMessage_ImageUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memberlogs", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "Moderations",
                columns: table => new
                {
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserBans = table.Column<string>(type: "TEXT", nullable: false),
                    SlowmodeLocks = table.Column<string>(type: "TEXT", nullable: false),
                    UserMutes = table.Column<string>(type: "TEXT", nullable: false),
                    ModLogChannel = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moderations", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "TriviaPlayers",
                columns: table => new
                {
                    UserId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionsCorrect = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionsIncorrect = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriviaPlayers", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    UserId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Balance = table.Column<int>(type: "INTEGER", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    LastDaily = table.Column<DateTime>(type: "TEXT", nullable: false)
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
