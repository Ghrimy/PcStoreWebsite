using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PCStore_API.Data;
using PCStore_API.Services.ProductServices;
using PCStore_Shared.Models;
using PCStore_Shared.Models.User;

namespace PCStore_API.Services.UserService;

public class UserService(PcStoreDbContext context, ILogger<ProductService> logger) : IUserService
{

    public async Task<UserLoginDto> LoginAsync(UserLoginDto user)
    {
        //Takes user input
        var existingUser = await context.UserLogin.Where(u => user.Username == u.Username).FirstOrDefaultAsync();
        
        if (existingUser == null)
            throw new ValidationException("User does not exist");

        if (existingUser.PasswordHash != null && !existingUser.PasswordHash.Equals(user.PasswordHash))
            throw new ValidationException("Invalid Password");

        await using var transactionAsync = await context.Database.BeginTransactionAsync();
        try
        {
            

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        //Checks if the user exists in the database
        //If the user exists, returns the user's id and token and role
        //If the user doesn't exist, returns an error message
        throw new NotImplementedException();
    }

    public Task<UserRegisterDto> RegisterAsync(UserRegisterDto user)
    {
        //Takes user input and checks if the user already exists in the database
        //If the user doesn't exist, creates a new user and returns the user's id and token and role
        //If the user already exists, returns an error message
        throw new NotImplementedException();
    }

    [Authorize(Roles = "User, Admin")]
    public Task<UserEditDto> EditAsync(UserEditDto user)
    {
        //Takes user input and checks if the user exists in the database
        //If the user exists, updates the user's information and returns the updated user and role
        //If the user doesn't exist, returns an error message
        throw new NotImplementedException();
    }

    [Authorize(Roles = "User, Admin")]
    public Task<UserRemoveDto> RemoveAsync(UserRemoveDto user)
    {
        
        //Takes user input and checks if the user exists in the database
        //If the user exists, sets a 30 day delete time before it deletes the user and returns timedate the user will be deleted
        //Also adds a cancel option to the delete request
        //If the user doesn't exist, returns an error message'
        throw new NotImplementedException();
    }
}