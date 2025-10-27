using PCStore_Shared.Models;
using PCStore_Shared.Models.User;

namespace PCStore_API.Services.UserService;

public interface IUserService
{
    public Task<LoginResultDto> LoginAsync(UserLoginDto user);
    public Task<LoginResultDto> RegisterAsync(UserRegisterDto user);
    public Task<UserEditDto> EditLoginAsync(UserEditDto user);
    public Task<UserDetailDto> EditDetailsAsync(UserEditDto user);
    public Task<UserRemoveDto> RemoveAsync(UserRemoveDto user);
}