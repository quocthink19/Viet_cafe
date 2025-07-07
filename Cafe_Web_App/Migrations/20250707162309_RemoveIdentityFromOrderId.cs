using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cafe_Web_App.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIdentityFromOrderId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Drop foreign key FK_payments_Orders_OrderId
            migrationBuilder.DropForeignKey(
                name: "FK_payments_Orders_OrderId",
                table: "payments"
            );

            // 2. Drop foreign key FK_OrderItems_Orders_OrderId
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems"
            );

            // 3. Drop primary key của Orders
            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders"
            );

            // 4. Drop cột Id cũ của Orders
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Orders"
            );

            // 5. Thêm lại cột Id mới KHÔNG identity
            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "Orders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L
            );

            // 6. Thêm lại primary key cho Orders
            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                column: "Id"
            );

            // 7. Thêm lại foreign key cho payments
            migrationBuilder.AddForeignKey(
                name: "FK_payments_Orders_OrderId",
                table: "payments",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            // 8. Thêm lại foreign key cho OrderItems
            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. Drop lại foreign key
            migrationBuilder.DropForeignKey(
                name: "FK_payments_Orders_OrderId",
                table: "payments"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems"
            );

            // 2. Drop lại primary key
            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders"
            );

            // 3. Drop cột Id mới
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Orders"
            );

            // 4. Thêm lại cột Id có identity
            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "Orders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L
            ).Annotation("SqlServer:ValueGenerationStrategy", Microsoft.EntityFrameworkCore.Metadata.SqlServerValueGenerationStrategy.IdentityColumn);

            // 5. Thêm lại primary key
            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                column: "Id"
            );

            // 6. Thêm lại foreign key payments
            migrationBuilder.AddForeignKey(
                name: "FK_payments_Orders_OrderId",
                table: "payments",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            // 7. Thêm lại foreign key OrderItems
            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}