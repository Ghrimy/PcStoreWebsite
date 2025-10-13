using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PCStore_API.Models.Order;

public class OrderItem
{
    // Properties
    [Key] public int OrderItemId { get; set; }
    public int ProductId { get; set; }

    [MaxLength(100)] public string? ProductName { get; set; } = string.Empty;
    [DataType(DataType.Currency)] [Precision(18, 2)] public decimal ProductPrice { get; set; }
    public int Quantity { get; set; }
    public int RefundedQuantity { get; set; }
    
    // Calculated Properties
    public decimal ProductTotal => ProductPrice * Quantity;

}