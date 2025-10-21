using PCStore_Shared.Models;
using PCStore_Shared.Models.User;

namespace PCStore_API.Services.UserService;

public interface IUserService
{
    public Task<UserLoginDto> LoginAsync(UserLoginDto user);
    public Task<UserRegisterDto> RegisterAsync(UserRegisterDto user);
    public Task<UserEditDto> EditAsync(UserEditDto user);
    public Task<UserRemoveDto> RemoveAsync(UserRemoveDto user);
}