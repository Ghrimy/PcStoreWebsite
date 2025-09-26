using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCStore_API.Data;
using PCStore_API.Extensions;
using PCStore_API.Models.ShoppingCart;
using PCStore_Shared;
using PCStore_Shared.Models;
using PCStore_Shared.Models.ShoppingCart;

namespace PCStore_API.Controllers;


[ApiController]
[Authorize(Roles = "User,Admin")]
[Route("api/[controller]")]
public class ShoppingCartController(PcStoreDbContext context, ILogger<ShoppingCartController> logger) : ControllerBase
{
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId)) return null;
        
        return userId;
    }

    private async Task<ShoppingCart> GetUserCartAsync(int userId)
    {
       
        var findCart = await context.ShoppingCart
            .Include(shoppingCart => shoppingCart.Items)
            .ThenInclude(shoppingCartItem => shoppingCartItem.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        return findCart;
    }

    private async Task<ShoppingCart> CreateCartForUserAsync(int userId)
    {
        

        var createCart =  new ShoppingCart()
        {
            UserId = userId,
            Items = new List<ShoppingCartItem>(),
            LastUpdated = DateTime.UtcNow,
        };
        
        context.ShoppingCart.Add(createCart);
        logger.LogInformation("Created new cart for user");
            
        await context.SaveChangesAsync();
        return createCart;
    }
    
    private async Task<ShoppingCart> GetOrCreateUserCartAsync(int userId)
    {
        return await GetUserCartAsync(userId) 
               ?? await CreateCartForUserAsync(userId);
    }


    [HttpGet]
    public async Task<ActionResult<ShoppingCartDto>> GetCart()
    { 
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            logger.LogInformation("User not found");
            return Unauthorized(ApiResponse<ShoppingCartDto>.FailureResponse("User not found", new List<string>{"No user found"}));
        }

        var cart = await GetOrCreateUserCartAsync(userId.Value);
        
        if (cart.LastUpdated < DateTime.UtcNow.AddDays(-30))
        {
            cart.Items.Clear();
            logger.LogInformation("User {UserId} cart {CartId} was cleared due to inactivity", userId.Value, cart.ShoppingCartId);
            await context.SaveChangesAsync();
        }

        if (cart.Items.Count == 0)
        {
            logger.LogInformation("Cart {cartId} is empty", cart.ShoppingCartId);
            return Ok(ApiResponse<ShoppingCartDto>.SuccessResponse(null, "Empty cart"));
        }
        
        return Ok(ApiResponse<ShoppingCartDto>.SuccessResponse(cart.ToDto(), "Request successful"));
    }
    

    [HttpPost("items")]
    public async Task<ActionResult<ShoppingCartDto>> AddItemToCart([FromBody] ShoppingCartItemDto item)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            logger.LogInformation("User not found");
            return Unauthorized(ApiResponse<ShoppingCartDto>.FailureResponse("User not found"));
        }

        var cart = await GetOrCreateUserCartAsync(userId.Value);
        
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        
        if(existingItem != null)
        {
            if (existingItem.Quantity + item.Quantity <= existingItem.Product.ProductStock)
            {
                logger.LogInformation("User {UserId} added {Quantity} of product {productId}", userId.Value, item.ProductId, item.Quantity);
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                logger.LogInformation("User {UserId} tried to add {Quantity} of product {ProductId}, but stock was exceeded.",
                    userId.Value, item.Quantity, item.ProductId);
                return BadRequest(ApiResponse<ShoppingCartDto>.FailureResponse("Stock was exceeded"));
                
            }
        }
        else
        {
            if (item.Quantity < 0)
            {
                logger.LogInformation("Product {ProductId} with Quantity: {Quantity}  can't be negative.", item.ProductId, item.Quantity);
                return BadRequest(ApiResponse<ShoppingCartDto>.FailureResponse("Quantity can't be negative"));
            }
            
            
            var product = await context.Products.FindAsync(item.ProductId);
            if (product == null)
            {
                logger.LogInformation("Product {ProductId} not found.", item.ProductId);
                return NotFound(ApiResponse<ShoppingCartDto>.FailureResponse("Product not found"));
            }
            cart.Items.Add(new ShoppingCartItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                ShoppingCartId = cart.ShoppingCartId,
                
            });
        }
        cart.LastUpdated = DateTime.UtcNow;
        
        await context.SaveChangesAsync();
        return Ok(ApiResponse<ShoppingCartDto>.SuccessResponse(cart.ToDto(), "Added product to cart"));
    }

    [HttpPut("items/{productId:int}/{amount:int}")]
    public async Task<ActionResult<ShoppingCartItemDto>> UpdateItemInCart(int productId, int amount)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            logger.LogInformation("User not found");
            return Unauthorized(ApiResponse<ShoppingCartDto>.FailureResponse("User not found", new List<string>{"No user found"}));
        }

        var cart = await GetOrCreateUserCartAsync(userId.Value);
        
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem == null)
        {
            logger.LogInformation("Item {ProductId} not found.", productId);
            return NotFound(ApiResponse<ShoppingCartDto>.FailureResponse("Product not found", new List<string>{$"No product {productId} found"}));
        }

        switch (amount)
        {
            case 0:
                logger.LogInformation("Product {ProductId} with Quantity: {Quantity}  can't be zero.", productId, amount);
                return BadRequest(ApiResponse<ShoppingCartDto>.FailureResponse("Quantity can't be zero"));
            case <= 0:
                logger.LogInformation("Product {ProductId} with Quantity: {Quantity}  can't be less than zero.", productId, amount);
                cart.Items.Remove(existingItem);
                break;
            default:
            {
                if (amount <= existingItem.Product.ProductStock)
                {
                    existingItem.Quantity = amount;
                }

                if (amount > existingItem.Product.ProductStock)
                {
                    logger.LogInformation("Product {ProductId} with Quantity: {Quantity}  can't be more than stock.", productId, amount);
                    return BadRequest(ApiResponse<ShoppingCartDto>.FailureResponse("Quantity can't be more than stock"));
                }

                break;
            }
        }
        cart.LastUpdated = DateTime.UtcNow;
        
        await context.SaveChangesAsync();
        return Ok(ApiResponse<ShoppingCartDto>.SuccessResponse(cart.ToDto(), "Updated item in cart"));
    }


    [HttpDelete("items/{productId:int}/{amount:int}")]
    public async Task<ActionResult> RemoveItemFromCart(int productId, int amount)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            logger.LogInformation("User not found");
            return Unauthorized(ApiResponse<ShoppingCartDto>.FailureResponse("User not found", new List<string>{"No user found"}));
        }
        
        var cart = await GetOrCreateUserCartAsync(userId.Value);
        
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem == null)
        {
            logger.LogInformation("Item {ProductId} not found.", productId);
            return NotFound(ApiResponse<ShoppingCartDto>.FailureResponse("Product not found", new List<string>{$"No product {productId} found"}));
        }

        switch (existingItem.Quantity - amount)
        {
            case <0:
                logger.LogInformation("Product {ProductId} with Quantity: {Quantity}  can't be negative.", productId, amount);
                return BadRequest(ApiResponse<ShoppingCartDto>.FailureResponse("Quantity can't be negative"));
                break;
            
            case 0:
                logger.LogInformation("User {UserId} removed product {ProductId} from cart", userId.Value, productId);
                cart.Items.Remove(existingItem);
                break;
            
            default:
                logger.LogInformation("User {UserId} removed {Quantity} of product {ProductId} from cart", userId.Value, amount, productId);
                existingItem.Quantity -= amount;
                break;
        }
        
        cart.LastUpdated = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return Ok(ApiResponse<ShoppingCartDto>.SuccessResponse(cart.ToDto(), "Removed item from cart"));
    }
    

    [HttpDelete]
    public async Task<ActionResult<ShoppingCartDto>> ClearCart()
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var cart = await GetOrCreateUserCartAsync(userId.Value);
        
        context.ShoppingCartItem.RemoveRange(cart.Items);
        cart.Items.Clear();
        cart.LastUpdated = DateTime.UtcNow;
        
        logger.LogInformation("User {UserId} cleared cart", userId.Value);
        
        await context.SaveChangesAsync();
        return Ok(ApiResponse<ShoppingCartDto>.SuccessResponse(cart.ToDto(), "Cleared cart"));
    }

}