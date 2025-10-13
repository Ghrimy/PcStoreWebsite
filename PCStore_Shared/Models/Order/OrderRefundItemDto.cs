using System.ComponentModel.DataAnnotations;
using PCStore_Shared.Models.Validation;

namespace PCStore_Shared.Models.Order;

public class OrderRefundItemDto
{
    [Required] public int ProductId { get; set; }
    public string? ProductName { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }
    [ValidateQuantity(1, 100)] public int Quantity { get; set; }
}
