using System.ComponentModel.DataAnnotations;

namespace PCStore_Shared.Models.Order;

public class OrderDto
{
    [Required] public int UserId { get; set; }

    [Required(ErrorMessage = "Order must have at least one item.")]
    [MinLength(1, ErrorMessage = "Order must have at least one item.")]
    public List<OrderItemDto> Items { get; set; } = new();

    [Range(0.01, double.MaxValue, ErrorMessage = "Total must be greater than 0")]
    public decimal OrderTotal { get; set; }

    [Required] public string? OrderStatus { get; set; }

    [Required] public DateTime OrderDate { get; set; }

    [Required] public DateTime? OrderDateUpdated { get; set; }
}