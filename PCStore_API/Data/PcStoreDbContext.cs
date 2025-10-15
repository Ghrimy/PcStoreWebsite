using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using PCStore_API.Models.Order;
using PCStore_API.Models.Product;
using PCStore_API.Models.ShoppingCart;
using PCStore_API.Models.User;
using PCStore_Shared;
using PCStore_Shared.Models.Product;

namespace PCStore_API.Data;

public class PcStoreDbContext(DbContextOptions<PcStoreDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ShoppingCart> ShoppingCart { get; set; }
    public DbSet<ShoppingCartItem> ShoppingCartItem { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    
    public DbSet<OrderRefundItem> OrderRefundItems { get; set; }
    public DbSet<OrderRefundHistory> OrderRefundHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Data Types
        modelBuilder.Entity<Product>().Property(p => p.ProductPrice).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Product>().Property(p => p.ProductDiscount).HasColumnType("decimal(18,2)");

        modelBuilder.Entity<ShoppingCart>().Property(s => s.TotalPrice).HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Order>().Property(o => o.OrderTotal).HasColumnType("decimal(18,2)");
        
        modelBuilder.Entity<OrderRefundHistory>().Property(o => o.RefundAmount).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<OrderRefundItem>().Property(o => o.ProductPrice).HasColumnType("decimal(18,2)");
        
        //Relationship configuration
        //User to shoppingcart
        modelBuilder.Entity<User>()
            .HasOne(u => u.ShoppingCart)
            .WithOne(s => s.User)
            .HasForeignKey<ShoppingCart>(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        //User to orders
        modelBuilder.Entity<User>()
            .HasMany(u => u.Orders)
            .WithOne(o => o.User)
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        //Order to order-items
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        //Order to order-refund-history
        modelBuilder.Entity<Order>()
            .HasOne(o => o.OrderRefundHistory)
            .WithOne(r => r.Order)
            .HasForeignKey<OrderRefundHistory>(r => r.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
        
        //Order-refund-history to order-refund-items
        modelBuilder.Entity<OrderRefundHistory>()
            .HasMany(r => r.RefundedItems)
            .WithOne(i => i.OrderRefundHistory)
            .HasForeignKey(r => r.OrderRefundHistoryId)
            .OnDelete(DeleteBehavior.Restrict);
        
        //Order-refund-history to user
        modelBuilder.Entity<OrderRefundHistory>()
            .HasOne(r => r.User)
            .WithMany(u => u.RefundHistories)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        //Order-refund-items to Order-item
        modelBuilder.Entity<OrderRefundItem>()
            .HasOne(i => i.OrderItem)
            .WithMany()
            .HasForeignKey(i => i.OrderItemId)
            .OnDelete(DeleteBehavior.Restrict);
        
        //Order-item to product
        modelBuilder.Entity<OrderItem>()
            .HasOne(i => i.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        //ShoppingCart to shoppingcart-items
        modelBuilder.Entity<ShoppingCart>()
            .HasMany(s => s.Items)
            .WithOne(i => i.ShoppingCart)
            .HasForeignKey(i => i.ShoppingCartId)
            .OnDelete(DeleteBehavior.Cascade);
        
        //Shoppingcart-items to product
        modelBuilder.Entity<ShoppingCartItem>()
            .HasOne(i => i.Product)
            .WithMany(p => p.ShoppingCartItems)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}