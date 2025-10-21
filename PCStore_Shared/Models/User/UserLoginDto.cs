using System.ComponentModel.DataAnnotations;

namespace PCStore_Shared.Models.User;

public class UserLoginDto
{
    [MaxLength(100)] [Required] public string? Username { get; set; }
    [Required] public string? PasswordHash { get; set; }
}