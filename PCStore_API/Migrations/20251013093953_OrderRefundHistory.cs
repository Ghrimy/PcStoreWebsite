using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCStore_API.Migrations
{
    /// <inheritdoc />
    public partial class OrderRefundHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_UserId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderItems",
                table: "OrderItems");

            migrationBuilder.AlterColumn<string>(
                name: "OrderStatus",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "OrderItems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Step 1: Add temporary column
            migrationBuilder.Sql("ALTER TABLE [OrderItems] ADD [ProductId_Temp] int NULL;");
            
            // Step 2: Copy data
            migrationBuilder.Sql("UPDATE [OrderItems] SET [ProductId_Temp] = [ProductId];");
            
            // Step 3: Drop the old IDENTITY column
            migrationBuilder.Sql("ALTER TABLE [OrderItems] DROP COLUMN [ProductId];");
            
            // Step 4: Recreate without IDENTITY
            migrationBuilder.Sql("ALTER TABLE [OrderItems] ADD [ProductId] int NOT NULL DEFAULT 0;");
            
            // Step 5: Copy data back
            migrationBuilder.Sql("UPDATE [OrderItems] SET [ProductId] = [ProductId_Temp];");
            
            // Step 6: Drop temporary column
            migrationBuilder.Sql("ALTER TABLE [OrderItems] DROP COLUMN [ProductId_Temp];");

            migrationBuilder.AddColumn<int>(
                name: "OrderItemId",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "RefundedQuantity",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderItems",
                table: "OrderItems",
                column: "OrderItemId");

            migrationBuilder.CreateTable(
                name: "OrderRefundHistory",
                columns: table => new
                {
                    OrderRefundHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RefundAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderRefundHistory", x => x.OrderRefundHistoryId);
                    table.ForeignKey(
                        name: "FK_OrderRefundHistory_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderRefundItem",
                columns: table => new
                {
                    OrderRefundItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProductPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OrderRefundHistoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderRefundItem", x => x.OrderRefundItemId);
                    table.ForeignKey(
                        name: "FK_OrderRefundItem_OrderRefundHistory_OrderRefundHistoryId",
                        column: x => x.OrderRefundHistoryId,
                        principalTable: "OrderRefundHistory",
                        principalColumn: "OrderRefundHistoryId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderRefundHistory_OrderId",
                table: "OrderRefundHistory",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderRefundItem_OrderRefundHistoryId",
                table: "OrderRefundItem",
                column: "OrderRefundHistoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderRefundItem");

            migrationBuilder.DropTable(
                name: "OrderRefundHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderItems",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "OrderItemId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "RefundedQuantity",
                table: "OrderItems");

            migrationBuilder.AlterColumn<string>(
                name: "OrderStatus",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            // Step 1: Add temporary column
            migrationBuilder.Sql("ALTER TABLE [OrderItems] ADD [ProductId_Temp] int NULL;");
            
            // Step 2: Copy data
            migrationBuilder.Sql("UPDATE [OrderItems] SET [ProductId_Temp] = [ProductId];");
            
            // Step 3: Drop the column
            migrationBuilder.Sql("ALTER TABLE [OrderItems] DROP COLUMN [ProductId];");
            
            // Step 4: Recreate with IDENTITY (we can't restore exact values with IDENTITY)
            migrationBuilder.Sql("ALTER TABLE [OrderItems] ADD [ProductId] int IDENTITY(1,1) NOT NULL;");
            
            // Step 5: Drop temporary column (can't copy back to IDENTITY column)
            migrationBuilder.Sql("ALTER TABLE [OrderItems] DROP COLUMN [ProductId_Temp];");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderItems",
                table: "OrderItems",
                column: "ProductId");


            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
