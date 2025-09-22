namespace PCStore_API.Models.ShoppingCart;

public class ShoppingCartDto
{
    public int UserId { get; set; }
    public List<ShoppingCartItemDto> ShoppingCartItems { get; set; } = new();
    public decimal? TotalPrice { get; set; }
    public DateTime LastUpdated { get; set; }
}