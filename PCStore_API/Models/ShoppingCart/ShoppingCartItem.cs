using System.ComponentModel.DataAnnotations;

namespace PCStore_API.Models.ShoppingCart;

public class ShoppingCartItem
{
    [Key]
    public int ShoppingCartItemId { get; set; }

    public int ShoppingCartId { get; set; }
    public ShoppingCart ShoppingCart { get; set; }

    public int ProductId { get; set; }
    public Product.Product Product { get; set; }

    public int Quantity { get; set; }
}