using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using PCStore_Shared.Models.User;

namespace PCStore.Components.Services;

public class CustomAuthStateProvider(AuthService authService, HttpClient http) : AuthenticationStateProvider
{

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (await authService.CheckAuthStatusAsync())
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "User"), // backend can return actual username
                new Claim(ClaimTypes.Role, "User")  // backend can return actual role
            };

            var identity = new ClaimsIdentity(claims, "Cookies");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }

        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        return new AuthenticationState(anonymous);
    }

    public void NotifyUserAuthentication(LoginResultDto loginResult)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, loginResult.Username),
            new Claim(ClaimTypes.Role, loginResult.UserCategory.ToString())
        };

        var identity = new ClaimsIdentity(claims, "Cookies");
        var user = new ClaimsPrincipal(identity);

        authService.IsAuthenticated = true;
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void NotifyUserLogout()
    {
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        authService.IsAuthenticated = false;
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
    }
}