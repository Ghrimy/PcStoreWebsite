using System.ComponentModel.DataAnnotations;

namespace PCStore_Shared.Models.User;

public class UserRegisterDto
{
    [Required] [MaxLength(100)] public string? Username { get; set; }
    [Required] public string? Password { get; set; }
    [Required] [MaxLength(100)] public string? Email { get; set; }
}