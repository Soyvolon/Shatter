using Microsoft.EntityFrameworkCore.Migrations;

namespace NitroSharp.Core.Migrations
{
    public partial class removeddepricatedculture : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Culture",
                table: "Configs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Culture",
                table: "Configs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
