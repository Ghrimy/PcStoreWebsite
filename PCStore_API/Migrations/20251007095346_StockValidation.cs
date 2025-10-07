using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCStore_API.Migrations
{
    /// <inheritdoc />
    public partial class StockValidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "ShoppingCart",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Products",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "ShoppingCart");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Products");
        }
    }
}
