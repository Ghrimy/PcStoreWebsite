using System.ComponentModel.DataAnnotations;

namespace PCStore_Shared;

public class ShoppingCartItem
{
    [Key]
    public int ShoppingCartItemId { get; set; }

    public int ShoppingCartId { get; set; }
    public Shoppingcart ShoppingCart { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int Quantity { get; set; }
}