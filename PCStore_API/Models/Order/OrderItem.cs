using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PCStore_API.Models.Order;

public class OrderItem
{
    [Key] public int OrderItemId { get; set; }
    public int ProductId { get; set; }
    
    public string ProductName { get; set; }

    [DataType(DataType.Currency)]
    [Precision(18, 2)]
    public decimal ProductPrice { get; set; }

    public int Quantity { get; set; }

    public int RefundedQuantity { get; set; }
    public decimal ProductTotal => ProductPrice * Quantity;
}