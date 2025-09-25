using System.ComponentModel.DataAnnotations;
using PCStore_Shared;

namespace PCStore_API.Models.ShoppingCart;

public class ShoppingCart
{
    [Key]
    public int ShoppingCartId { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public List<ShoppingCartItem> Items { get; set; } = new();

    public decimal TotalPrice => Items.Sum(i => i.Product.ProductPrice * i.Quantity);
    
    public DateTime LastUpdated { get; set; }
}