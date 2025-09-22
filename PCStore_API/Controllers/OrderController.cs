using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCStore_API.Data;
using PCStore_API.Models.Order;
using PCStore_Shared;

namespace PCStore_API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class OrderController(PcStoreDbContext context, ILogger<OrderController> logger) : ControllerBase
{
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId)) return null;
        
        return userId;
    }

    private async Task<Shoppingcart> GetUserCartAsync(int userId)
    {
       
        var findCart = await context.ShoppingCart
            .Include(shoppingCart => shoppingCart.Items)
            .ThenInclude(shoppingCartItem => shoppingCartItem.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        return findCart;
    }
    
    [Authorize(Roles = "User")]
    [HttpPost("checkout")]
    public async Task<ActionResult<OrderDto>> Checkout()
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var cart = await GetUserCartAsync(userId.Value);
        if (cart == null)
        {
            logger.LogInformation("This {UserId} has no cart", userId.Value);
            return BadRequest(new { error = "The user has no cart." });
        }

        foreach (var item in cart.Items.Where(item => item.Quantity > item.Product.ProductStock))
        {
            logger.LogInformation("User {UserId} tried to add {Quantity} of product {ProductId}, but stock was exceeded.",
                userId.Value, item.Quantity, item.ProductId);
            return BadRequest(new { error = "Quantity exceeds available stock." });
        }
        
        foreach (var item in cart.Items)
        {
            item.Product.ProductStock -= item.Quantity;
        }

        var cartItems = cart.Items.Select(item => new OrderItem()
        {
            ProductId = item.ProductId,
            ProductName = item.Product.ProductName,
            ProductPrice = item.Product.ProductPrice,
            Quantity = item.Quantity,
        }).ToList();

        var createOrder = new Order()
        {
            UserId = userId.Value,
            Items = cartItems,
            OrderDate = DateTime.UtcNow,
            OrderDateUpdated = DateTime.UtcNow,
            OrderStatus = "Ordered",
            OrderTotal = cart.Items.Sum(i => i.Quantity * i.Product.ProductPrice)
        };
        
        var orderDto = new OrderDto
        {
            OrderId = createOrder.OrderId,
            UserId = createOrder.UserId,
            Items = createOrder.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ProductPrice = i.ProductPrice,
                Quantity = i.Quantity
            }).ToList(),
            OrderTotal = createOrder.OrderTotal,
            OrderStatus = createOrder.OrderStatus,
            OrderDate = createOrder.OrderDate,
            OrderDateUpdated = createOrder.OrderDateUpdated
        };

        context.Orders.Add(createOrder);
        context.ShoppingCartItem.RemoveRange(cart.Items);
        cart.Items.Clear();
        await context.SaveChangesAsync();
        return Ok(orderDto);
    }
}