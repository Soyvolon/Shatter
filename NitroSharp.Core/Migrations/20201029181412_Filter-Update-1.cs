using Microsoft.EntityFrameworkCore.Migrations;

namespace NitroSharp.Core.Migrations
{
    public partial class FilterUpdate1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BypassFilters",
                table: "Filters",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BypassFilters",
                table: "Filters");
        }
    }
}
