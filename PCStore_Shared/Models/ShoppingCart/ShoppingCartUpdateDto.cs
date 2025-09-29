using System.ComponentModel.DataAnnotations;

namespace PCStore_Shared.Models.ShoppingCart;

public class ShoppingCartUpdateDto
{
    [Required]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}