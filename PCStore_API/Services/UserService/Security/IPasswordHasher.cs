namespace PCStore_API.Services.UserService.Security;

public interface IPasswordHasher
{
    byte[] GenerateSalt();
    byte[] HashPassword(string password, byte[] salt);
    bool VerifyPassword(string password, byte[] salt, byte[] hashedPassword);
}