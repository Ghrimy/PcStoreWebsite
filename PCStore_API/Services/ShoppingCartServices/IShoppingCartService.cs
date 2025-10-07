using PCStore_API.Models.ShoppingCart;
using PCStore_Shared.Models.ShoppingCart;

namespace PCStore_API.Services.ShoppingCartServices;

public interface IShoppingCartService
{
    public Task<ShoppingCart> GetShoppingCartAsync(int userId);
    public Task<ShoppingCartDto> AddToShoppingCartAsync(int userId, ShoppingCartAddDto item);
    public Task<ShoppingCartDto> RemoveFromShoppingCartAsync(int userId, List<ShoppingCartRemoveDto> items);
    public Task<ShoppingCartDto> UpdateShoppingCartAsync(int userId, List<ShoppingCartUpdateDto> items);
    public Task<ShoppingCartDto> ClearShoppingCartAsync(int userId);
    public decimal CalculateTotal(ShoppingCart cart);
}