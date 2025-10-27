using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCStore_API.ApiResponse;
using PCStore_API.Services.UserService;
using PCStore_Shared.Models.User;

namespace PCStore_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController(IUserService userService, ILogger logger) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResultDto>> LoginAsync(UserLoginDto user)
    {
        var userLogin = await userService.LoginAsync(user);

        if (!userLogin.Success)
        {
            logger.LogWarning("Failed login attempt for {Username}", user.Username);
            return Unauthorized(ApiResponse<LoginResultDto>.FailureResponse("Invalid credentials"));
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userLogin.Username),
            new Claim(ClaimTypes.Role, userLogin.UserCategory.ToString())
        };
        
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = true });
        logger.LogInformation("User {Username} logged in successfully", userLogin.Username);
        return Ok(ApiResponse<LoginResultDto>.SuccessResponse(userLogin, "Login successful"));
    }

    [HttpPost("logout")]
    public async Task<ActionResult<LoginResultDto>> LogoutAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(ApiResponse<LoginResultDto>.SuccessResponse(null, "Logout successful"));
        
    }
    
} 