using System.ComponentModel.DataAnnotations;

namespace PCStore_Shared;

public class Shoppingcart
{
    [Key]
    public int ShoppingCartId { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public List<ShoppingCartItem> Items { get; set; } = new();

    public decimal TotalPrice => Items.Sum(i => i.Product.ProductPrice * i.Quantity);
}