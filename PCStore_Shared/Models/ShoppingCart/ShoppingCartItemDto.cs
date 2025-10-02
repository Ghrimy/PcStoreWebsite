namespace PCStore_Shared.Models.ShoppingCart;

public class ShoppingCartItemDto
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public int Quantity { get; set; }

    public decimal ProductPrice { get; set; }

    public decimal? TotalPrice => Math.Round(ProductPrice * Quantity, 2);
}