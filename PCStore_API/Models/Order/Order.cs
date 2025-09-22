using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PCStore_Shared;

public class Order
{
    [Key]
    public int OrderId { get; set; }
    public int UserId { get; set; }
    
    public User User { get; set; }

    public List<OrderItem> Items { get; set; } = new();

    [DataType(DataType.Currency)]
    [Precision(18, 2)]
    public decimal OrderTotal { get; set; }
    public string? OrderStatus { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? OrderDateUpdated { get; set; }
}