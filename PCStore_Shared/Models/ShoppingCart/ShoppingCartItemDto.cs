using System.ComponentModel.DataAnnotations;

namespace PCStore_Shared.Models.ShoppingCart;

public class ShoppingCartItemDto
{
    [Required]
    public int ProductId { get; set; }
    
    [Required]
    public string? ProductName { get; set; }
    
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal? ProductPrice { get; set; }
    
    public decimal? TotalPrice => ProductPrice * Quantity; // computed
}