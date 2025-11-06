namespace PCStore_Shared.Models.User;

public enum UserCategory
{
    User,
    Employee,
    Admin
}

public class LoginResultDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public int? UserId { get; set; }
    public string? Username { get; set; }
    
    public string? Email { get; set; }
    public string? Token { get; set; }
    public UserCategory UserCategory { get; set; }
}