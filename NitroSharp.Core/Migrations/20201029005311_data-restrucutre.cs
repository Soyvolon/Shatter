using Microsoft.EntityFrameworkCore.Migrations;

namespace NitroSharp.Migrations
{
    public partial class datarestrucutre : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JoinDmMessage",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "MemberlogChannel",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "UserBans",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "JoinMessage_ImageUrl",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "JoinMessage_IsEmbed",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "JoinMessage_IsImage",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "JoinMessage_Message",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "LeaveMessage_ImageUrl",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "LeaveMessage_IsEmbed",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "LeaveMessage_IsImage",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "LeaveMessage_Message",
                table: "Configs");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Filters");

            migrationBuilder.DropTable(
                name: "Memberlogs");

            migrationBuilder.DropTable(
                name: "Moderations");

            migrationBuilder.AddColumn<string>(
                name: "JoinDmMessage",
                table: "Configs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MemberlogChannel",
                table: "Configs",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserBans",
                table: "Configs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JoinMessage_ImageUrl",
                table: "Configs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "JoinMessage_IsEmbed",
                table: "Configs",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "JoinMessage_IsImage",
                table: "Configs",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JoinMessage_Message",
                table: "Configs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LeaveMessage_ImageUrl",
                table: "Configs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LeaveMessage_IsEmbed",
                table: "Configs",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LeaveMessage_IsImage",
                table: "Configs",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LeaveMessage_Message",
                table: "Configs",
                type: "text",
                nullable: true);
        }
    }
}
