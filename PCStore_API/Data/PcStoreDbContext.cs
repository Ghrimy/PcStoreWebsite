using Microsoft.EntityFrameworkCore;
using PCStore_API.Models.Order;
using PCStore_API.Models.Product;
using PCStore_API.Models.ShoppingCart;
using PCStore_Shared;
namespace PCStore_API.Data;


public class PcStoreDbContext(DbContextOptions<PcStoreDbContext> options) : DbContext(options)
{

    public DbSet<Product?> Products { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Shoppingcart> ShoppingCart { get; set; }
    public DbSet<ShoppingCartItem> ShoppingCartItem { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

}
