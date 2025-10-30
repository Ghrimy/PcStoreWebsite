using PCStore_Shared.Models.User;
using PCStore_Shared.Models.Validation;

namespace PCStore.Components.Services;

public class AuthService(HttpClient _http)
{
    public bool IsAuthenticated { get; set; }

    public async Task<ApiResponseAccount<LoginResultDto>> LoginAsync(UserLoginDto user)
    {
        var response = await _http.PostAsJsonAsync("api/Account/login", user);
        var content = await response.Content.ReadFromJsonAsync<ApiResponseAccount<LoginResultDto>>();

        if (content != null && content.Success)
            IsAuthenticated = true;

        return content ?? new ApiResponseAccount<LoginResultDto>();
    }
    
    public async Task<ApiResponseAccount<LoginResultDto>> RegisterAsync(UserRegisterDto user)
    {
        var response = await _http.PostAsJsonAsync("api/Account/register", user);
        var content = await response.Content.ReadFromJsonAsync<ApiResponseAccount<LoginResultDto>>();
        return content ?? new ApiResponseAccount<LoginResultDto>();
    }

    public async Task<ApiResponseAccount<LoginResultDto>> LogoutAsync()
    {
        var response = await _http.PostAsync("api/Account/logout", null);
        var content = await response.Content.ReadFromJsonAsync<ApiResponseAccount<LoginResultDto>>();

        IsAuthenticated = false;

        return content ?? new ApiResponseAccount<LoginResultDto>();
    }

    public async Task<bool> CheckAuthStatusAsync()
    {
        try
        {
            var response = await _http.GetFromJsonAsync<LoginResultDto>("api/Account/authstatus");
            IsAuthenticated = response != null;
        }
        catch
        {
            IsAuthenticated = false;
        }

        return IsAuthenticated;
    }
}