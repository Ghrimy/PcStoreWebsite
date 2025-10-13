using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCStore_API.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureRefundRelationShips : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderRefundHistory_Orders_OrderId",
                table: "OrderRefundHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderRefundItem_OrderRefundHistory_OrderRefundHistoryId",
                table: "OrderRefundItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderRefundItem",
                table: "OrderRefundItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderRefundHistory",
                table: "OrderRefundHistory");

            migrationBuilder.RenameTable(
                name: "OrderRefundItem",
                newName: "OrderRefundItems");

            migrationBuilder.RenameTable(
                name: "OrderRefundHistory",
                newName: "OrderRefundHistories");

            migrationBuilder.RenameIndex(
                name: "IX_OrderRefundItem_OrderRefundHistoryId",
                table: "OrderRefundItems",
                newName: "IX_OrderRefundItems_OrderRefundHistoryId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderRefundHistory_OrderId",
                table: "OrderRefundHistories",
                newName: "IX_OrderRefundHistories_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderRefundItems",
                table: "OrderRefundItems",
                column: "OrderRefundItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderRefundHistories",
                table: "OrderRefundHistories",
                column: "OrderRefundHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderRefundHistories_Orders_OrderId",
                table: "OrderRefundHistories",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderRefundItems_OrderRefundHistories_OrderRefundHistoryId",
                table: "OrderRefundItems",
                column: "OrderRefundHistoryId",
                principalTable: "OrderRefundHistories",
                principalColumn: "OrderRefundHistoryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderRefundHistories_Orders_OrderId",
                table: "OrderRefundHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderRefundItems_OrderRefundHistories_OrderRefundHistoryId",
                table: "OrderRefundItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderRefundItems",
                table: "OrderRefundItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderRefundHistories",
                table: "OrderRefundHistories");

            migrationBuilder.RenameTable(
                name: "OrderRefundItems",
                newName: "OrderRefundItem");

            migrationBuilder.RenameTable(
                name: "OrderRefundHistories",
                newName: "OrderRefundHistory");

            migrationBuilder.RenameIndex(
                name: "IX_OrderRefundItems_OrderRefundHistoryId",
                table: "OrderRefundItem",
                newName: "IX_OrderRefundItem_OrderRefundHistoryId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderRefundHistories_OrderId",
                table: "OrderRefundHistory",
                newName: "IX_OrderRefundHistory_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderRefundItem",
                table: "OrderRefundItem",
                column: "OrderRefundItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderRefundHistory",
                table: "OrderRefundHistory",
                column: "OrderRefundHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderRefundHistory_Orders_OrderId",
                table: "OrderRefundHistory",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderRefundItem_OrderRefundHistory_OrderRefundHistoryId",
                table: "OrderRefundItem",
                column: "OrderRefundHistoryId",
                principalTable: "OrderRefundHistory",
                principalColumn: "OrderRefundHistoryId");
        }
    }
}
