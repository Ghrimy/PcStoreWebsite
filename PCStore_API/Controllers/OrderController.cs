using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCStore_API.ApiResponse;
using PCStore_API.Data;
using PCStore_API.Extensions;
using PCStore_API.Models.Order;
using PCStore_API.Models.ShoppingCart;
using PCStore_API.Services.OrderServices;
using PCStore_Shared.Models.Order;

namespace PCStore_API.Controllers;

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
    [HttpGet("orders/{id:int}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int orderId)
    {
        //Gets the user id
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<OrderDto>.FailureResponse("User not found"));
        
        var orderDto = await orderService.GetOrderByIdAsync(userId.Value, orderId);
        return Ok(ApiResponse<OrderDto>.SuccessResponse(orderDto, "Order found"));
    }
}