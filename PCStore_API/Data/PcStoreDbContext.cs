using Microsoft.EntityFrameworkCore;
using PCStore_Shared;

namespace PCStore_API.Data;

public class PcStoreDbContext(DbContextOptions<PcStoreDbContext> options) : DbContext(options)
{

    public DbSet<Product?> Products { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Shoppingcart> ShoppingCart { get; set; }
    public DbSet<ShoppingCartItem> ShoppingCartItem { get; set; }
}