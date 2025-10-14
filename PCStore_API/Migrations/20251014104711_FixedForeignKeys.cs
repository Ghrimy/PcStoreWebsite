using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCStore_API.Migrations
{
    /// <inheritdoc />
    public partial class FixedForeignKeysAddedSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderId1",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartItem_Products_ProductId1",
                table: "ShoppingCartItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartItem_ShoppingCart_ShoppingCartId",
                table: "ShoppingCartItem");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCartItem_ProductId1",
                table: "ShoppingCartItem");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_OrderId1",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                table: "ShoppingCartItem");

            migrationBuilder.DropColumn(
                name: "OrderId1",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Users",
                newName: "Username");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                table: "OrderItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartItem_ShoppingCart_ShoppingCartId",
                table: "ShoppingCartItem",
                column: "ShoppingCartId",
                principalTable: "ShoppingCart",
                principalColumn: "ShoppingCartId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartItem_ShoppingCart_ShoppingCartId",
                table: "ShoppingCartItem");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "UserName");

            migrationBuilder.AddColumn<int>(
                name: "ProductId1",
                table: "ShoppingCartItem",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderId1",
                table: "OrderItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartItem_ProductId1",
                table: "ShoppingCartItem",
                column: "ProductId1");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId1",
                table: "OrderItems",
                column: "OrderId1",
                unique: true,
                filter: "[OrderId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderId1",
                table: "OrderItems",
                column: "OrderId1",
                principalTable: "Orders",
                principalColumn: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                table: "OrderItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartItem_Products_ProductId1",
                table: "ShoppingCartItem",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartItem_ShoppingCart_ShoppingCartId",
                table: "ShoppingCartItem",
                column: "ShoppingCartId",
                principalTable: "ShoppingCart",
                principalColumn: "ShoppingCartId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
