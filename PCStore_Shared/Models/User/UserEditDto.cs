using System.ComponentModel.DataAnnotations;

namespace PCStore_Shared.Models.User;

public class UserEditDto
{
    [MaxLength(100)] public string? Username { get; set; }
    [MaxLength(100)] public string? Email { get; set; }
    [Required] public string? Password { get; set; }
}