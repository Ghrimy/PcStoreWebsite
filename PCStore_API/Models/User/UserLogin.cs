using System.ComponentModel.DataAnnotations;

namespace PCStore_API.Models.User;

public enum UserCategory
{
    User,
    Employee,
    Admin
}

public class UserLogin
{
    //Login properties
    [Key] public int UserId { get; set; }
    [MaxLength(100)] public string? Username { get; set; }
    [MaxLength(100)] public string? Email { get; set; }
    
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
    
    public UserCategory UserCategory { get; set; }

    public UserDetails UserDetails { get; set; } = null!;
}