using System.ComponentModel.DataAnnotations;
using PCStore_Shared.Models.Validation;

namespace PCStore_Shared.Models.Order;

public class RefundItemDto
{
    [Required] public int ProductId { get; set; }

    [ValidateQuantity(1, 100)] public int Quantity { get; set; }
}