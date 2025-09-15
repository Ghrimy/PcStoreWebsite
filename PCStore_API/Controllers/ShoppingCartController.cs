using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCStore_API.Data;
using PCStore_API.Models;
using PCStore_Shared;

namespace PCStore_API.Controllers;


[ApiController]
[Authorize(Roles = "User")]
[Route("api/[controller]")]
public class ShoppingCartController(PcStoreDbContext context) : ControllerBase
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

    

    [HttpGet]
    public async Task<ActionResult<ShoppingCartDto>> GetCart()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("User ID claim is missing or invalid.");
        }
        var cart = await context.ShoppingCart
            .Include(shoppingCart => shoppingCart.Items)
            .ThenInclude(shoppingCartItem => shoppingCartItem.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);
        if(cart == null) return NotFound();

        var cartDto = MapCartToDto(cart);
        return Ok(cartDto);
    }
    

    [HttpPost("items")]
    public async Task<ActionResult<ShoppingCartDto>> AddItemToCart([FromBody] ShoppingCartItemDto item)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("User ID claim is missing or invalid.");
        }
        var cart = await context.ShoppingCart
            .Include(shoppingCart => shoppingCart.Items)
            .ThenInclude(shoppingCartItem => shoppingCartItem.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);
        if(cart == null) return NotFound();

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if(existingItem != null)
        {
            existingItem.Quantity += item.Quantity;
        }
        else
        {
            cart.Items.Add(new ShoppingCartItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                ShoppingCartId = cart.ShoppingCartId,
            });
        }

        if (item.Quantity < 0)
        {
            return BadRequest("Quantity cannot be negative.");
        }

        await context.SaveChangesAsync();
        var cartDto = MapCartToDto(cart);
        return Ok(cartDto);
    }


    [HttpDelete("items/{productId:int}/{amount:int}")]
    public async Task<ActionResult> RemoveItemFromCart(int productId, int amount)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("User ID claim is missing or invalid.");
        }
        var cart = await context.ShoppingCart
            .Include(shoppingCart => shoppingCart.Items)
            .ThenInclude(shoppingCartItem => shoppingCartItem.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);
        
        if(cart == null) return NotFound();
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if(existingItem == null) return NotFound();

        if (existingItem != null)
        {
            existingItem.Quantity -= amount;
            if (existingItem.Quantity <= 0)
            {
                cart.Items.Remove(existingItem);
            }
        }

        if (existingItem.Quantity < 0)
        {
            return BadRequest("Quantity cannot be negative.");
        }
        
        var cartDto = MapCartToDto(cart);
        await context.SaveChangesAsync();
        return Ok(cartDto);
        
    }
    

    [HttpDelete]
    public async Task<ActionResult<ShoppingCartDto>> ClearCart()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("User ID claim is missing or invalid.");
        }
        var cart = await context.ShoppingCart
            .Include(shoppingCart => shoppingCart.Items)
            .ThenInclude(shoppingCartItem => shoppingCartItem.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);
        
        if(cart == null) return NotFound();
        
        context.ShoppingCartItem.RemoveRange(cart.Items);
        var cartDto = MapCartToDto(cart);
        await context.SaveChangesAsync();
        return Ok(cartDto);
    }
}