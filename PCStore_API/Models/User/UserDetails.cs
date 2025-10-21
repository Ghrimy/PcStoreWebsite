using System.ComponentModel.DataAnnotations;
using PCStore_API.Models.Order;

namespace PCStore_API.Models.User;

public class UserDetails
{
    
    // Properties
    [Key] public int UserDetailsId { get; set; }
    public int UserId { get; set; }

    
    // personal information
    [MaxLength(100)] public string? FirstName { get; set; }
    [MaxLength(100)] public string? LastName { get; set; }
    [MaxLength(100)] public string? Email { get; set; }
    [MaxLength(100)] public string? PhoneNumber { get; set; }
    [MaxLength(100)] public string? Address { get; set; }
    [MaxLength(100)] public string? City { get; set; }
    [MaxLength(100)] public string? Country { get; set; }
    public int ZipCode { get; set; }
    
    
    //Navigation
    public ShoppingCart.ShoppingCart ShoppingCart { get; set; } = null!;
    public List<Order.Order> Orders { get; set; } = null!;
    public List<OrderRefundHistory> RefundHistories { get; set; } = new();
    public UserLogin UserLogin { get; set; } = null!;
    
}