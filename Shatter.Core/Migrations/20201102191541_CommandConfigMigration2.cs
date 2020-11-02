using Microsoft.EntityFrameworkCore.Migrations;

namespace Shatter.Core.Migrations
{
    public partial class CommandConfigMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisabledGroups",
                table: "Configs");

            migrationBuilder.AddColumn<string>(
                name: "DisabledModules",
                table: "Configs",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisabledModules",
                table: "Configs");

            migrationBuilder.AddColumn<string>(
                name: "DisabledGroups",
                table: "Configs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
