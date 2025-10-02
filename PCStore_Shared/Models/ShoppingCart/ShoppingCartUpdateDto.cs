using System.ComponentModel.DataAnnotations;
using PCStore_Shared.Models.Validation;


namespace PCStore_Shared.Models.ShoppingCart;

public class ShoppingCartUpdateDto
{
    [Required]
    public int ProductId { get; set; }
    
    [Required]
    [ValidateQuantity(1, 100)]
    public int NewQuantity { get; set; }
}