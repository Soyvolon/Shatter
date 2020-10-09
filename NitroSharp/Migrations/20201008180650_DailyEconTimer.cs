using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitroSharp.Migrations
{
    public partial class DailyEconTimer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastDaily",
                table: "Wallets",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastDaily",
                table: "Wallets");
        }
    }
}
