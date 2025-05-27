using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cafe_Web_App.Migrations
{
    /// <inheritdoc />
    public partial class updateCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "Customizes");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderItemId",
                table: "Customizes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    UnitPrice = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customizes_OrderItemId",
                table: "Customizes",
                column: "OrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customizes_OrderItem_OrderItemId",
                table: "Customizes",
                column: "OrderItemId",
                principalTable: "OrderItem",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customizes_OrderItem_OrderItemId",
                table: "Customizes");

            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_Customizes_OrderItemId",
                table: "Customizes");

            migrationBuilder.DropColumn(
                name: "OrderItemId",
                table: "Customizes");

            migrationBuilder.AddColumn<string>(
                name: "Temperature",
                table: "Customizes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
