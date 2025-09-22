namespace PCStore_API.Models.Order;

public class OrderDto
{
    public int OrderId { get; set; }
    public int UserId { get; set; }

    public List<OrderItemDto> Items { get; set; } = new();

    public decimal OrderTotal { get; set; }
    public string? OrderStatus { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? OrderDateUpdated { get; set; }   
}