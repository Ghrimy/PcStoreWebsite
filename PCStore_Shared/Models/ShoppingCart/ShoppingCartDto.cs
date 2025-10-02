using System.ComponentModel.DataAnnotations;

namespace PCStore_Shared.Models.ShoppingCart;

public class ShoppingCartDto
{
    public List<ShoppingCartItemDto> ShoppingCartItems { get; set; } = new();

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal TotalPrice => ShoppingCartItems.Sum(i => i.TotalPrice ?? 0);

    [Required] public DateTime LastUpdated { get; set; }
}