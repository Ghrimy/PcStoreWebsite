using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PCStore_API.Data;
using PCStore_API.Models.Order;
using PCStore_API.Models.ShoppingCart;
using PCStore_Shared;

namespace PCStore_API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class OrderController(PcStoreDbContext context, ILogger<OrderController> logger) : ControllerBase
{
    private static OrderDto MapToOrderDto(Order order) => new OrderDto
    {
        OrderId = order.OrderId,
        UserId = order.UserId,
        Items = order.Items.Select(i => new OrderItemDto
        {
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            ProductPrice = i.ProductPrice,
            Quantity = i.Quantity
        }).ToList(),
        OrderTotal = order.OrderTotal,
        OrderStatus = order.OrderStatus,
        OrderDate = order.OrderDate,
        OrderDateUpdated = order.OrderDateUpdated
        
    };
    private int? GetCurrentUserId()
    {
        //Gets the user id
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId)) return null;
        
        return userId;
    }

    private async Task<Shoppingcart> GetUserCartAsync(int userId)
    {
       //Gets the user cart
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
        //Gets the user id
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        //Checks if the user has a cart
        var cart = await GetUserCartAsync(userId.Value);
        if (cart == null)
        {
            logger.LogInformation("This {UserId} has no cart", userId.Value);
            return BadRequest(new { error = "The user has no cart." });
        }

        //Checks if the stock is enough
        foreach (var item in cart.Items.Where(item => item.Quantity > item.Product.ProductStock))
        {
            logger.LogInformation("User {UserId} tried to add {Quantity} of product {ProductId}, but stock was exceeded.",
                userId.Value, item.Quantity, item.ProductId);
            return BadRequest(new { error = "Quantity exceeds available stock." });
        }
        
        await using var transactionAsync = await context.Database.BeginTransactionAsync();

        try
        {

            //Updates the stock
            foreach (var item in cart.Items)
            {
                item.Product.ProductStock -= item.Quantity;
            }

            //Creates the order items
            var cartItems = cart.Items.Select(item => new OrderItem()
            {
                ProductId = item.ProductId,
                ProductName = item.Product.ProductName,
                ProductPrice = item.Product.ProductPrice,
                Quantity = item.Quantity,
            }).ToList();

            //Creates the order
            var createOrder = new Order()
            {
                UserId = userId.Value,
                Items = cartItems,
                OrderDate = DateTime.UtcNow,
                OrderDateUpdated = DateTime.UtcNow,
                OrderStatus = "Ordered",
                OrderTotal = cart.Items.Sum(i => i.Quantity * i.Product.ProductPrice)
            };

            context.Orders.Add(createOrder);
            context.ShoppingCartItem.RemoveRange(cart.Items);
            cart.Items.Clear();

            await context.SaveChangesAsync();
            logger.LogInformation("User {UserId} placed order {OrderId}", userId.Value, createOrder.OrderId);
            await transactionAsync.CommitAsync();
            
            //Creates the order and returns it to the front end
            var orderDto = MapToOrderDto(createOrder);
            return Ok(orderDto);
        }
        catch (Exception ex)
        {
            await transactionAsync.RollbackAsync();
            logger.LogError(ex, "Checkout failed for user {UserId}", userId.Value);
            return StatusCode(500, "An error occurred while processing your order.");
        }
    }

    [Authorize(Roles = "User")]
    [HttpGet("orders")]
    public async Task<ActionResult<OrderDto>> GetOrders()
    {
        var userId = GetCurrentUserId();
        var findOrders = await context.Orders
            .Where(i => i.UserId == userId)
            .Include(o => o.Items)
            .OrderByDescending(i => i.OrderDate).ToListAsync();


        if (!findOrders.Any())
        {
            logger.LogInformation("User has no orders");
            return NoContent();
        }
        
        var orderDtos = findOrders.Select(order => new OrderDto
        {
            OrderId = order.OrderId,
            UserId = order.UserId,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ProductPrice = i.ProductPrice,
                Quantity = i.Quantity
            }).ToList(),
            OrderTotal = order.OrderTotal,
            OrderStatus = order.OrderStatus,
            OrderDate = order.OrderDate,
            OrderDateUpdated = order.OrderDateUpdated
        }).ToList();

        return Ok(orderDtos);
    }
    
    [Authorize(Roles = "User")]
    [HttpGet("orders/{id:int}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        var userId = GetCurrentUserId();
        var findOrder = await context.Orders
            .Where(i => i.OrderId == id && i.UserId == userId)
            .Include(o => o.Items)
            .OrderByDescending(i => i.OrderDate).FirstOrDefaultAsync();
        if (findOrder == null)
        {
            logger.LogInformation("User has no order with id {OrderId}", id);
            return NotFound();
        }
        
        var orderDto = MapToOrderDto(findOrder);

        return Ok(orderDto);
    }
}