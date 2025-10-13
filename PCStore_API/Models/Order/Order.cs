using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PCStore_API.Models.Order;

public class Order
{
    // Properties
    [Key] public int OrderId { get; set; }
    public int UserId { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    
    [DataType(DataType.Currency)]
    [Precision(18, 2)] 
    public decimal OrderTotal { get; set; }
    
    [MaxLength(100)]
    public string? OrderStatus { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? OrderDateUpdated { get; set; }
    public OrderRefundHistory OrderRefundHistory { get; set; } = new();


}