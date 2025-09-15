using System.ComponentModel.DataAnnotations;

namespace PCStore_API.Models;

public class ShoppingCartItemDto
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal? ProductPrice { get; set; }
    public decimal? TotalPrice => ProductPrice * Quantity; // computed
}