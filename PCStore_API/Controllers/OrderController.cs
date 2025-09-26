using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCStore_API.Data;
using PCStore_API.Models.Order;
using PCStore_API.Models.ShoppingCart;
using PCStore_API.Extensions;
using PCStore_Shared.Models.Order;

namespace PCStore_API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class OrderController(PcStoreDbContext context, ILogger<OrderController> logger) : ControllerBase
{

    private int? GetCurrentUserId()
    {
        //Gets the user id
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId)) return null;
        
        //returns user id
        return userId;
    }

    private async Task<ShoppingCart> GetUserCartAsync(int userId)
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

            //Removes the cart items and clears the cart
            context.Orders.Add(createOrder);
            context.ShoppingCartItem.RemoveRange(cart.Items);
            cart.Items.Clear();

            //Saves the changes
            await context.SaveChangesAsync();
            logger.LogInformation("User {UserId} placed order {OrderId}", userId.Value, createOrder.OrderId);
            //Commits the transaction
            await transactionAsync.CommitAsync();
            
            //Creates the order and returns it to the front end
            var dto = createOrder.ToDto();
            return Ok(dto);
        }
        catch (Exception ex)
        {
            //If something goes wrong, rolls back the transaction
            await transactionAsync.RollbackAsync();
            logger.LogError(ex, "Checkout failed for user {UserId}", userId.Value);
            return StatusCode(500, "An error occurred while processing your order.");
        }
    }

    [Authorize(Roles = "User")]
    [HttpGet("orders")]
    public async Task<ActionResult<OrderDto>> GetOrders()
    {
        //Gets the user id
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();
        
        //Gets the user orders
        var findOrders = await context.Orders
            .Where(i => i.UserId == userId)
            .Include(o => o.Items)
            .ToListAsync();

        //Checks if the user has orders
        if (!findOrders.Any())
        {
            logger.LogInformation("User has no orders");
            return Ok(new List<OrderDto>());
        }
      
        //Maps the orders to the DTOs
        var orderDtos = findOrders.Select(i => i.ToDto()).ToList();

        return Ok(orderDtos);
    }
    
    [Authorize(Roles = "User")]
    [HttpGet("orders/{id:int}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        //Gets the user id
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();
        
        //Gets the user order
        var findOrder = await context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(i => i.OrderId == id && i.UserId == userId);
        
        //Checks if the user has an order with the given id
        if (findOrder == null)
        {
            logger.LogInformation("User has no order with id {OrderId}", id);
            return NotFound();
        }
        //Maps the order to the DTO
        var orderDto = findOrder.ToDto();

        return Ok(orderDto);
    }
}