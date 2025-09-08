using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PCStore_API.Migrations
{
    /// <inheritdoc />
    public partial class SeedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "ProductBrand", "ProductCategory", "ProductDescription", "ProductImage", "ProductName", "ProductPrice", "ProductStock" },
                values: new object[,]
                {
                    { 1, null, 5, null, null, "RTX 4080", 1299.99m, 5 },
                    { 2, null, 6, null, null, "Intel i9 CPU", 599.99m, 10 },
                    { 3, null, 6, null, null, "AMD Ryzen 9 5900X", 1499.99m, 10 },
                    { 5, null, 2, null, null, "Asrock AM4 B450M", 1299.99m, 5 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5);
        }
    }
}
