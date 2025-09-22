using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PCStore_API.Models.Order;

public class OrderItem
{
    [Key]
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    
    [DataType(DataType.Currency)]
    [Precision(18, 2)]
    public decimal ProductPrice { get; set; }
    public int Quantity { get; set; }

    [DataType(DataType.Currency)]
    [Precision(18, 2)]
    public decimal ProductTotal => ProductPrice * Quantity;
}