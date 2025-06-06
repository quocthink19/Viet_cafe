using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cafe_Web_App.Migrations
{
    /// <inheritdoc />
    public partial class fix_customize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ice",
                table: "Customizes");

            migrationBuilder.DropColumn(
                name: "Milk",
                table: "Customizes");

            migrationBuilder.DropColumn(
                name: "Sugar",
                table: "Customizes");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Customizes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "Customizes");

            migrationBuilder.AddColumn<string>(
                name: "Ice",
                table: "Customizes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Milk",
                table: "Customizes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sugar",
                table: "Customizes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
