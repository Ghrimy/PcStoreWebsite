using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCStore_API.ApiResponse;
using PCStore_API.Services.OrderServices;
using PCStore_Shared.Models.Order;

namespace PCStore_API.Controllers.Order;

[ApiController]
[Route("api/[controller]")]
public class OrderController(IOrderService orderService) : ControllerBase
{
    private int? GetCurrentUserId()
    {
        //Gets the user id
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId)) return null;

        //returns user id
        return userId;
    }

    [Authorize(Roles = "User")]
    [HttpPost("checkout")]
    public async Task<ActionResult<OrderDto>> Checkout()
    {
        //Gets the user id
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<OrderDto>.FailureResponse("User not found"));

        var newOrder = await orderService.CreateOrder(userId.Value);
        return Ok(ApiResponse<OrderDto>.SuccessResponse(newOrder, "Order created successfully"));
    }

    [Authorize(Roles = "User")]
    [HttpGet("orders")]
    public async Task<ActionResult<List<OrderDto>>> GetOrders()
    {
        //Gets the user id
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<OrderDto>.FailureResponse("User not found"));

        var orders = await orderService.GetOrdersByUserIdAsync(userId.Value);
        return Ok(ApiResponse<List<OrderDto>>.SuccessResponse(orders, "Orders found"));
    }

    [Authorize(Roles = "User")]
    [HttpGet("orders/{orderId:int}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int orderId)
    {
        //Gets the user id
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<OrderDto>.FailureResponse("User not found"));
        
        var orderDto = await orderService.GetOrderByIdAsync(userId.Value, orderId);
        return Ok(ApiResponse<OrderDto>.SuccessResponse(orderDto, "Order found"));
    }

    [Authorize(Roles = "User")]
    [HttpPost("orders/{orderId:int}/refund-history")]
    public async Task<ActionResult<RefundItemDto>> RefundOrder(int orderId, [FromBody] List<RefundItemDto> items)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<OrderDto>.FailureResponse("User not found"));

        var refundDto = await orderService.RefundOrder(userId.Value, orderId, items);
        return Ok(ApiResponse<OrderDto>.SuccessResponse(refundDto, "Order refunded successfully"));
    }
}