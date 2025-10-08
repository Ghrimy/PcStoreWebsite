using PCStore_API.Models.Order;
using PCStore_Shared.Models.Order;

namespace PCStore_API.Services.OrderServices;

public interface IOrderService
{
    public Task<OrderDto> CreateOrder(int userId);
    public Task<OrderDto> GetOrderByIdAsync(int userId, int orderId);
    public Task<List<OrderDto>> GetOrdersByUserIdAsync(int userId);
    public Task<OrderDto> RefundOrder(int userId, int orderId, List<RefundItemDto> items);
    
}