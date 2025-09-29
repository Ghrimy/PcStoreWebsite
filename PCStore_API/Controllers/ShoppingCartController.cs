using System.ComponentModel.DataAnnotations;
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
    public async Task<ActionResult<ShoppingCartDto>> AddItemToCart([FromBody][Required] ShoppingCartUpdateDto item)
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

    [HttpPut("items")]
    public async Task<ActionResult<ShoppingCartDto>> UpdateItemsInCart([FromBody][Required] List<ShoppingCartUpdateDto> items)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            logger.LogInformation("User not found");
            return Unauthorized(ApiResponse<ShoppingCartDto>.FailureResponse("User not found", new List<string>{"No user found"}));
        }

        var cart = await GetOrCreateUserCartAsync(userId.Value);

        foreach (var product in items)
        {
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == product.ProductId);
            if (existingItem == null)
            {
                logger.LogInformation("Item {ProductId} not found.", product.ProductId);
                return NotFound(ApiResponse<ShoppingCartDto>.FailureResponse("Product not found", new List<string>{$"No product {product.ProductId} found"}));
            }
            
            
            switch (product.Quantity)
            {
                case 0:
                    logger.LogInformation("Product {ProductId} with Quantity: {Quantity}  can't be zero.", product.ProductId, product.Quantity);
                    return BadRequest(ApiResponse<ShoppingCartDto>.FailureResponse("Quantity can't be zero"));
                case <= 0:
                    logger.LogInformation("Product {ProductId} with Quantity: {Quantity}  can't be less than zero.", product.ProductId, product.Quantity);
                    cart.Items.Remove(existingItem);
                    break;
                default:
                {
                    if (product.Quantity <= existingItem.Product.ProductStock)
                    {
                        existingItem.Quantity = product.Quantity;
                    }

                    if (product.Quantity > existingItem.Product.ProductStock)
                    {
                        logger.LogInformation("Product {ProductId} with Quantity: {Quantity}  can't be more than stock.", product.ProductId, product.Quantity);
                        return BadRequest(ApiResponse<ShoppingCartDto>.FailureResponse("Quantity can't be more than stock"));
                    }

                    break;
                }
            }
            cart.LastUpdated = DateTime.UtcNow;
        }
        await context.SaveChangesAsync();
        return Ok(ApiResponse<ShoppingCartDto>.SuccessResponse(cart.ToDto(), "Updated item in cart"));
    }


    [HttpDelete("items")]
    public async Task<ActionResult> RemoveItemFromCart([FromBody] [Required] List<ShoppingCartUpdateDto> item)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            logger.LogInformation("User not found");
            return Unauthorized(
                ApiResponse<ShoppingCartDto>.FailureResponse("User not found", new List<string> { "No user found" }));
        }

        var cart = await GetOrCreateUserCartAsync(userId.Value);


        foreach (var product in item)
        {
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == product.ProductId);
            if (existingItem == null)
            {
                logger.LogInformation("Item {ProductId} not found.", product.ProductId);
                return NotFound(ApiResponse<ShoppingCartDto>.FailureResponse("Product not found",
                    new List<string> { $"No product {product.ProductId} found" }));
            }

            switch (existingItem.Quantity - product.Quantity)
            {
                case < 0:
                    logger.LogInformation("Product {ProductId} with Quantity: {Quantity}  can't be negative.",
                        product.ProductId, product.Quantity);
                    return BadRequest(ApiResponse<ShoppingCartDto>.FailureResponse("Quantity can't be negative"));
                    break;

                case 0:
                    logger.LogInformation("User {UserId} removed product {ProductId} from cart", userId.Value,
                        product.ProductId);
                    cart.Items.Remove(existingItem);
                    break;

                default:
                    logger.LogInformation("User {UserId} removed {Quantity} of product {ProductId} from cart",
                        userId.Value, product.Quantity, product.ProductId);
                    existingItem.Quantity -= product.Quantity;
                    break;
            }
            cart.LastUpdated = DateTime.UtcNow;
        }
        
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