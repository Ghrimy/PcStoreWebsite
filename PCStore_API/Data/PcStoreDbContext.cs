using Microsoft.EntityFrameworkCore;
using PCStore_API.Models.Order;
using PCStore_API.Models.Product;
using PCStore_API.Models.ShoppingCart;
using PCStore_Shared;

namespace PCStore_API.Data;

public class PcStoreDbContext(DbContextOptions<PcStoreDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ShoppingCart> ShoppingCart { get; set; }
    public DbSet<ShoppingCartItem> ShoppingCartItem { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().Property(p => p.ProductPrice).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Product>().Property(p => p.ProductDiscount).HasColumnType("decimal(18,2)");

        modelBuilder.Entity<ShoppingCart>().Property(s => s.TotalPrice).HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Order>().Property(o => o.OrderTotal).HasColumnType("decimal(18,2)");
    }
}