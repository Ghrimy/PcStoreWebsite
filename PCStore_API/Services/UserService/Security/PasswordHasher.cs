using System.Security.Cryptography;

namespace PCStore_API.Services.UserService.Security;

public class PasswordHasher : IPasswordHasher
{
    private const int Iterations = 100000;
    private const int KeySize = 64;
    
    public byte[] GenerateSalt() => RandomNumberGenerator.GetBytes(64);
    public byte[] HashPassword(string password, byte[] salt) => Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
    
    public bool VerifyPassword(string password, byte[] salt, byte[] hashedPassword)
    {
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
        return CryptographicOperations.FixedTimeEquals(hash, hashedPassword);
    }
}