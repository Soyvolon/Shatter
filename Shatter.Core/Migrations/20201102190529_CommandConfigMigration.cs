using Microsoft.EntityFrameworkCore.Migrations;

namespace Shatter.Core.Migrations
{
    public partial class CommandConfigMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivatedCommands",
                table: "Configs",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DisabledCommands",
                table: "Configs",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DisabledGroups",
                table: "Configs",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivatedCommands",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "DisabledCommands",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "DisabledGroups",
                table: "Configs");
        }
    }
}
