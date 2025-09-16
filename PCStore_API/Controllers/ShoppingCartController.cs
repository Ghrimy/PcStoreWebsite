using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId)) return null;
        
        return userId;
    }

    

    [HttpGet]
    public async Task<ActionResult<ShoppingCartDto>> GetCart()
    {
        var userIdClaim = GetCurrentUserId();
        
        var cart = await context.ShoppingCart
            .Include(shoppingCart => shoppingCart.Items)
            .ThenInclude(shoppingCartItem => shoppingCartItem.Product)
            .FirstOrDefaultAsync(c => c.UserId == userIdClaim);
        if(cart == null) return NotFound();

        var cartDto = MapCartToDto(cart);
        return Ok(cartDto);
    }
    

    [HttpPost("items")]
    public async Task<ActionResult<ShoppingCartDto>> AddItemToCart([FromBody] ShoppingCartItemDto item)
    {
        var userIdClaim = GetCurrentUserId();
        
        var cart = await context.ShoppingCart
            .Include(shoppingCart => shoppingCart.Items)
            .ThenInclude(shoppingCartItem => shoppingCartItem.Product)
            .FirstOrDefaultAsync(c => c.UserId == userIdClaim);
        if(cart == null) return NotFound();
        if(cart.Items.IsNullOrEmpty())
        {
            cart.Items = new List<ShoppingCartItem>();
        }

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if(existingItem != null)
        {
            if (existingItem.Quantity + item.Quantity <= context.Products.Find(item.ProductId).ProductStock)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                return BadRequest("Quantity exceeds stock.");
            }
        }
        else
        {
            if (item.Quantity < 0) return BadRequest("Quantity cannot be less than zero.");
            var product = await context.Products.FindAsync(item.ProductId);
            if (product == null) return NotFound();
            cart.Items.Add(new ShoppingCartItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                ShoppingCartId = cart.ShoppingCartId,
            });
        }
        
        var cartDto = MapCartToDto(cart);
        cartDto.TotalPrice = cartDto.ShoppingCartItems.Sum(i => i.Quantity * i.ProductPrice);
        await context.SaveChangesAsync();
        return Ok(cartDto);
    }


    [HttpDelete("items/{productId:int}/{amount:int}")]
    public async Task<ActionResult> RemoveItemFromCart(int productId, int amount)
    {
        var userIdClaim = GetCurrentUserId();
        
        var cart = await context.ShoppingCart
            .Include(shoppingCart => shoppingCart.Items)
            .ThenInclude(shoppingCartItem => shoppingCartItem.Product)
            .FirstOrDefaultAsync(c => c.UserId == userIdClaim);
        
        if(cart == null) return NotFound();
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if(existingItem == null) return NotFound();

        if (existingItem != null)
        {
            if (existingItem.Quantity - amount < 0)
            {
                return BadRequest("Can't have less than zero items in the cart.");
            }
            else if (existingItem.Quantity - amount == 0)
            {
                cart.Items.Remove(existingItem);
            }
            else
            {
                existingItem.Quantity -= amount;
            }
        }


        var cartDto = MapCartToDto(cart);
        cartDto.TotalPrice = cartDto.ShoppingCartItems.Sum(i => i.Quantity * i.ProductPrice);
        await context.SaveChangesAsync();
        return Ok(cartDto);
        
    }
    

    [HttpDelete]
    public async Task<ActionResult<ShoppingCartDto>> ClearCart()
    {
        var userIdClaim = GetCurrentUserId();
        
        var cart = await context.ShoppingCart
            .Include(shoppingCart => shoppingCart.Items)
            .ThenInclude(shoppingCartItem => shoppingCartItem.Product)
            .FirstOrDefaultAsync(c => c.UserId == userIdClaim);
        
        if(cart == null) return NotFound();
        
        context.ShoppingCartItem.RemoveRange(cart.Items);
        await context.SaveChangesAsync();
        return NoContent();
    }
}