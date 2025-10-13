using System.ComponentModel.DataAnnotations;

namespace PCStore_API.Models.Order;

public class OrderRefundItem
{
    // Properties
    [Key] public int OrderRefundItemId { get; set; }
    public int OrderRefundHistoryId { get; set; }
    public int OrderItemId { get; set; }
    public int ProductId { get; set; }
    [MaxLength(100)] public string? ProductName { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }
    public int Quantity { get; set; }
    [MaxLength(100)] public string? Reason { get; set; }


    public OrderRefundHistory OrderRefundHistory { get; set; }
    public OrderItem OrderItem { get; set; }

}