using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PCStore_API.ApiResponse;
using PCStore_API.Services.UserService;
using PCStore_Shared.Models.User;

namespace PCStore_API.Controllers.Account;

[ApiController]
[Route("api/[controller]")]
public class AccountController(IUserService userService, IConfiguration config) : ControllerBase
{
    private string GenerateJwtToken(string username, object role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim("role", role.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: "https://localhost:5005",
            audience: "https://localhost:5002",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    
    [HttpPost("login")]
    public async Task<ActionResult<LoginResultDto>> LoginAsync(UserLoginDto user)
    {
        var userLogin = await userService.LoginAsync(user);

        if (!userLogin.Success)
            return Unauthorized(ApiResponse<LoginResultDto>.FailureResponse("Invalid credentials"));

        var token = GenerateJwtToken(userLogin.Username, userLogin.UserCategory);

        var result = new LoginResultDto
        {
            Username = userLogin.Username,
            UserCategory = userLogin.UserCategory,
            Token = token,
        };

        return Ok(ApiResponse<LoginResultDto>.SuccessResponse(result, "Login successful"));
    }

    [HttpPost("logout")]
    public async Task<ActionResult<LoginResultDto>> LogoutAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(ApiResponse<LoginResultDto>.SuccessResponse(null, "Logout successful"));
        
    }

    [HttpGet("authstatus")]
    public async Task<ActionResult<LoginResultDto>> AuthStatusAsync()
    {
        var userInfo = await userService.AuthenticateAsync(User);

        if (userInfo is null)
            return Unauthorized(ApiResponse<LoginResultDto>.FailureResponse("Not Authenticated"));

        return Ok(ApiResponse<LoginResultDto>.SuccessResponse(userInfo, "Authenticated"));
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<LoginResultDto>> RegisterAsync(UserRegisterDto user)
    {
        var result = await userService.RegisterAsync(user);
        return Ok(ApiResponse<LoginResultDto>.SuccessResponse(result, "Registration successful"));
    }
    
} 