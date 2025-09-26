using System.ComponentModel.DataAnnotations;
namespace PCStore_Shared.Models.ShoppingCart;

public class ShoppingCartDto
{
   
    [Required]
    public List<ShoppingCartItemDto> ShoppingCartItems { get; set; } = new();
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal TotalPrice { get; set; }
    
    [Required]
    public DateTime LastUpdated { get; set; }
}