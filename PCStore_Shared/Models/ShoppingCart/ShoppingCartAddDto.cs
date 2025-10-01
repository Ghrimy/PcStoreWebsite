using System.ComponentModel.DataAnnotations;
using PCStore_Shared.Models.ShoppingCart.Validation;

namespace PCStore_Shared.Models.ShoppingCart;

public class ShoppingCartAddDto
{
    [Required]
    public int ProductId { get; set; }
    
    [Range(1, int.MaxValue)]
    [ValidateQuantity]
    public int Quantity { get; set; }
}