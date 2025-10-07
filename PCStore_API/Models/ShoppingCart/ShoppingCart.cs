using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PCStore_Shared;

namespace PCStore_API.Models.ShoppingCart;

public class ShoppingCart
{
    [Key] public int ShoppingCartId { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public List<ShoppingCartItem> Items { get; set; } = new();
    
    public decimal TotalPrice { get; private set; }

    public DateTime LastUpdated { get; set; }
    
}