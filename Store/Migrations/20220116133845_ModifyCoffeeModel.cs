using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeStore.Migrations
{
    public partial class ModifyCoffeeModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ProducedAt",
                table: "Coffees",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProducedAt",
                table: "Coffees");
        }
    }
}
