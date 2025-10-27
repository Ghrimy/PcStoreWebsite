using System.ComponentModel.DataAnnotations;

namespace PCStore_Shared.Models.User;

public class UserDetailDto
{
// personal information
    [MaxLength(100)] public string? FirstName { get; set; }
    [MaxLength(100)] public string? LastName { get; set; }
    [MaxLength(100)] public string? Email { get; set; }
    [MaxLength(100)] public string? PhoneNumber { get; set; }
    [MaxLength(100)] public string? Address { get; set; }
    [MaxLength(100)] public string? City { get; set; }
    [MaxLength(100)] public string? Country { get; set; }
    public int ZipCode { get; set; }
}