namespace PCStore_API.Models.Order;

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public decimal ProductPrice { get; set; }
    public int Quantity { get; set; }

    public decimal ProductTotal => ProductPrice * Quantity;
}