using System.ComponentModel.DataAnnotations;
using PCStore_API.Models.User;


namespace PCStore_API.Models.ShoppingCart;

public class ShoppingCart
{
    [Key] public int ShoppingCartId { get; set; }
    public int UserId { get; set; }
    public List<ShoppingCartItem> Items { get; set; } = new();
    public decimal TotalPrice { get; internal set; }
    public DateTime LastUpdated { get; set; }
    
    //Navigation
    public UserDetails UserDetails { get; set; }
    
}