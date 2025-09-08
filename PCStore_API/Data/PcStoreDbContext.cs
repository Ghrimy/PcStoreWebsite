using Microsoft.EntityFrameworkCore;
using PCStore_Shared;
namespace PCStore_API.Data;


public class PcStoreDbContext(DbContextOptions<PcStoreDbContext> options) : DbContext(options)
{

    public DbSet<Product?> Products { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Shoppingcart> ShoppingCart { get; set; }
    public DbSet<ShoppingCartItem> ShoppingCartItem { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product { ProductId = 1, ProductName = "RTX 4080", ProductPrice = 1299.99m, ProductStock = 5,  ProductCategory = ProductCategory.PcGpu },
            new Product { ProductId = 2, ProductName = "Intel i9 CPU", ProductPrice = 599.99m, ProductStock = 10, ProductCategory = ProductCategory.PcCpu },
            new Product { ProductId = 3, ProductName = "AMD Ryzen 9 5900X", ProductPrice = 1499.99m, ProductStock = 10 , ProductCategory = ProductCategory.PcCpu },
            new Product { ProductId = 5, ProductName = "Asrock AM4 B450M", ProductPrice = 1299.99m, ProductStock = 5, ProductCategory = ProductCategory.PcMotherboard }
        );
    }
}
