using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cafe_Web_App.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCartItemsUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CartItems_CustomizeId", // Xóa index UNIQUE cũ gây lỗi
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_CartId", // Xóa luôn index cũ trên CartId nếu có
                table: "CartItems");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId_CustomizeId",
                table: "CartItems",
                columns: new[] { "CartId", "CustomizeId" },
                unique: true);
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CartItems_CartId_CustomizeId",
                table: "CartItems");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CustomizeId",  // Tạo lại index UNIQUE cũ nếu cần
                table: "CartItems",
                column: "CustomizeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId",
                table: "CartItems",
                column: "CartId");
        }
    }
}
