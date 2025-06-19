using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cafe_Web_App.Migrations
{
    /// <inheritdoc />
    public partial class updateCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "phoneNumber",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "phoneNumber",
                table: "Customers");
        }
    }
}
