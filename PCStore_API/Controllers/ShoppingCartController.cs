using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PCStore_API.Data;
using PCStore_API.Models;
using PCStore_Shared;

namespace PCStore_API.Controllers;


[ApiController]
[Authorize(Roles = "User,Admin")]
[Route("api/[controller]")]
public class ShoppingCartController(PcStoreDbContext context, ILogger<ShoppingCartController> logger) : ControllerBase
{
    private static ShoppingCartDto MapCartToDto(Shoppingcart cart) =>
        new ShoppingCartDto
        {
            UserId = cart.UserId,
            ShoppingCartItems = cart.Items.Select(i => new ShoppingCartItemDto
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                ProductPrice = i.Product.ProductPrice,
                ProductName = i.Product.ProductName
            }).ToList(),
            TotalPrice = cart.Items.Sum(i => i.Quantity * i.Product.ProductPrice)
        };

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

    private async Task<Shoppingcart> CreateCartForUserAsync(int userId)
    {
        

        var createCart =  new Shoppingcart()
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
    
    private async Task<Shoppingcart> GetOrCreateUserCartAsync(int userId)
    {
        return await GetUserCartAsync(userId) 
               ?? await CreateCartForUserAsync(userId);
    }


    [HttpGet]
    public async Task<ActionResult<ShoppingCartDto>> GetCart()
    { 
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var cart = await GetOrCreateUserCartAsync(userId.Value);
        
        if (cart.LastUpdated < DateTime.UtcNow.AddDays(-30))
        {
            cart.Items.Clear();
            logger.LogInformation("User {UserId} cart {CartId} was cleared due to inactivity", userId.Value, cart.ShoppingCartId);
            await context.SaveChangesAsync();
        }
  
        if (cart.Items.Count == 0) return NoContent();
        
        var cartDto = MapCartToDto(cart);
        return Ok(cartDto);
    }
    

    [HttpPost("items")]
    public async Task<ActionResult<ShoppingCartDto>> AddItemToCart([FromBody] ShoppingCartItemDto item)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var cart = await GetOrCreateUserCartAsync(userId.Value);
        
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        
        if(existingItem != null)
        {
            if (existingItem.Quantity + item.Quantity <= existingItem.Product.ProductStock)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                logger.LogInformation("User {UserId} tried to add {Quantity} of product {ProductId}, but stock was exceeded.",
                    userId.Value, item.Quantity, item.ProductId);
                return BadRequest(new { error = "Quantity exceeds available stock." });
                
            }
        }
        else
        {
            if (item.Quantity < 0)
            {
                logger.LogInformation("Product {ProductId} with Quantity: {Quantity}  can't be negative.", item.ProductId, item.Quantity);
                return BadRequest(new { error = "Quantity cannot be negative." });
            }
            
            
            var product = await context.Products.FindAsync(item.ProductId);
            if (product == null)
            {
                logger.LogInformation("Product {ProductId} not found.", item.ProductId);
                return NotFound();
            }
            cart.Items.Add(new ShoppingCartItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                ShoppingCartId = cart.ShoppingCartId,
                
            });
        }
        
        cart.LastUpdated = DateTime.UtcNow;
        var cartDto = MapCartToDto(cart);
        
        await context.SaveChangesAsync();
        return Ok(cartDto);
    }

    [HttpPut("items/{productId:int}/{amount:int}")]
    public async Task<ActionResult<ShoppingCartItemDto>> UpdateItemInCart(int productId, int amount)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var cart = await GetOrCreateUserCartAsync(userId.Value);
        
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if(existingItem == null) return NotFound();

        switch (amount)
        {
            case 0:
                logger.LogInformation("Product {ProductId} with Quantity: {Quantity}  can't be zero.", productId, amount);
                return BadRequest(new { error = "Quantity cannot be zero." });
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
                    return BadRequest(new { error = "Quantity exceeds available stock." });
                }

                break;
            }
        }
        
        cart.LastUpdated = DateTime.UtcNow;
        var cartDto = MapCartToDto(cart);
        
        await context.SaveChangesAsync();
        return Ok(cartDto);
    }


    [HttpDelete("items/{productId:int}/{amount:int}")]
    public async Task<ActionResult> RemoveItemFromCart(int productId, int amount)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();
        
        var cart = await GetOrCreateUserCartAsync(userId.Value);
        
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if(existingItem == null) return NotFound();

        switch (existingItem.Quantity - amount)
        {
            case <0:
                logger.LogInformation("Product {ProductId} with Quantity: {Quantity}  can't be negative.", productId, amount);
                return BadRequest(new { error = "Quantity cannot be negative." });
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
        var cartDto = MapCartToDto(cart);

        await context.SaveChangesAsync();
        return Ok(cartDto);
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
        var cartDto = MapCartToDto(cart);
        
        await context.SaveChangesAsync();
        return Ok(cartDto);
    }
}