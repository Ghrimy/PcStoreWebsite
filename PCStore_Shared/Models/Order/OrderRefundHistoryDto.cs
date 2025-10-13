using System.ComponentModel.DataAnnotations;

namespace PCStore_Shared.Models.Order;

public class OrderRefundHistoryDto
{
    public int OrderId { get; set; }
    public DateTime Date { get; set; }
    public decimal RefundAmount { get; set; }
    [MaxLength(100)] public string? Reason { get; set; }
    public List<OrderRefundItemDto> RefundedItems { get; set; } = new();
}