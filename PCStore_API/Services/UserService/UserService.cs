using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PCStore_API.Data;
using PCStore_API.Extensions;
using PCStore_API.Models.User;
using PCStore_API.Services.ProductServices;
using PCStore_API.Services.UserService.Security;
using PCStore_Shared.Models;
using PCStore_Shared.Models.User;
using UserCategory = PCStore_Shared.Models.User.UserCategory;

namespace PCStore_API.Services.UserService;

public class UserService(PcStoreDbContext context, IPasswordHasher passwordHasher, ILogger<ProductService> logger) : IUserService
{

    public async Task<LoginResultDto> LoginAsync(UserLoginDto user)
    {
        //Checks if user exists in the database
        var existingUser = await context.UserLogin
            .FirstOrDefaultAsync(u => u.Username == user.Username || u.Email == user.Email);

        if (existingUser == null) throw new ValidationException("User not found");
        if(user.Password == null) throw new ValidationException("Password cannot be empty");

        var result = passwordHasher.VerifyPassword(user.Password!, existingUser.PasswordSalt, existingUser.PasswordHash);
        if (!result)
            return new LoginResultDto
            {
                Success = false,
                Message = "Invalid password"
            };
            
        return new LoginResultDto
        {
            Success = true,
            Message = "Login successful",
            UserId = existingUser.UserId,
            Username = existingUser.Username,
            UserCategory = existingUser.UserCategory
        };
    }

    public async Task<LoginResultDto> RegisterAsync(UserRegisterDto user)
    {
        var existingUser = await context.UserLogin
            .FirstOrDefaultAsync(u => u.Username == user.Username || u.Email == user.Email);

        if (existingUser == null) throw new ValidationException("User not found");
        if(user.Password == null) throw new ValidationException("Password cannot be empty");

        await using var transactionAsync = await context.Database.BeginTransactionAsync();
        try
        {
            var userSalt = passwordHasher.GenerateSalt();
            var userPasswordHash = passwordHasher.HashPassword(user.Password, userSalt);

            var newUser = new UserLogin()
            {
                Email = user.Email,
                PasswordHash = userPasswordHash,
                PasswordSalt = userSalt,
                Username = user.Username,
                UserCategory = UserCategory.User,
            
            };
            
            await context.UserLogin.AddAsync(newUser);
            logger.LogInformation("Created new user with id {UserId}", newUser.UserId);
            await transactionAsync.CommitAsync();
            await context.SaveChangesAsync();
            return newUser.LoginResultToDto();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [Authorize(Roles = "User, Admin")]
    public Task<UserEditDto> EditLoginAsync(UserEditDto user)
    {
        throw new NotImplementedException();
    }

    [Authorize(Roles = "User, Admin")]
    public Task<UserDetailDto> EditDetailsAsync(UserEditDto user)
    {
        throw new NotImplementedException();
    }

    [Authorize(Roles = "User, Admin")]
    public Task<UserRemoveDto> RemoveAsync(UserRemoveDto user)
    {
        throw new NotImplementedException();
    }
}