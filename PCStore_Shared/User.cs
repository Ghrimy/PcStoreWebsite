using System.ComponentModel.DataAnnotations;

namespace PCStore_Shared;

public enum UserCategory
{
    User,
    Employee,
    Admin
}
public class User
{
    [Key]
    public int UserId { get; set; }
    
    // login
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public UserCategory UserCategory { get; set; }
    
    // personal information
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? ZipCode { get; set; }
}