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
    public string? PasswordHash { get; set; }
    public UserCategory UserCategory { get; set; }

    public UserDetails UserDetails { get; set; } = null!;
}