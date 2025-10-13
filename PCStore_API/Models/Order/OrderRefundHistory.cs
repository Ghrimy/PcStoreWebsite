using System.ComponentModel.DataAnnotations;

namespace PCStore_API.Models.Order;

public class OrderRefundHistory
{
    [Key] public int OrderRefundHistoryId { get; set; }

    public int UserId { get; set; }
    public int OrderId { get; set; }

    public DateTime Date { get; set; }
    public decimal RefundAmount { get; set; }
    [MaxLength(100)] public string? Reason { get; set; }
    public List<OrderRefundItem> RefundedItems { get; set; } = new();
    
    // Navigation
    public Order Order { get; set; } = null!;
    public List<OrderRefundItem> OrderRefundItems = new();
    public User.User User { get; set; }

}