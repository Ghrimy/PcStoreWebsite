using PCStore_API.Models.User;
using PCStore_Shared.Models.User;

namespace PCStore_API.Extensions;

public static class UserExtension
{
    public static LoginResultDto LoginResultToDto(this UserLogin userLogin)
    {
        return new LoginResultDto
        {
            Email = userLogin.Email,
            Username = userLogin.Username,
            UserId = userLogin.UserId,
            UserCategory = userLogin.UserCategory
        };
    }
    
    public static void UpdateLoginToDto(this UserLogin userLogin, UserEditDto userEditDto)
    {
        if(userEditDto.Email != null) userLogin.Email = userEditDto.Email;
        if(userEditDto.Username != null) userLogin.Username = userEditDto.Username;
    }

}