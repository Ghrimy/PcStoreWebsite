using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCStore_API.Data;
using PCStore_API.Models;
using PCStore_Shared;

namespace PCStore_API.Controllers;


[ApiController]
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
    
    [HttpGet("{userId}")]
    public async Task<ActionResult<ShoppingCartDto>> GetCart(int userId)
    {
        var cart = await context.ShoppingCart
            .Include(shoppingCart => shoppingCart.Items).ThenInclude(shoppingCartItem => shoppingCartItem.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);
        if(cart == null) return NotFound();

        var cartDto = MapCartToDto(cart);
        return Ok(cartDto);
    }

    [HttpPost("{userId}/items")]
    public async Task<ActionResult<ShoppingCartDto>> AddItemToCart(int userId, [FromBody] ShoppingCartItemDto item)
    {
        var cart = await context.ShoppingCart
            .Include(shoppingCart => shoppingCart.Items).ThenInclude(shoppingCartItem => shoppingCartItem.Product)
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

        await context.SaveChangesAsync();
        var cartDto = MapCartToDto(cart);
        return Ok(cartDto);
    }


    [HttpDelete("{userId}/items/{productId}/{amount}")]
    public async Task<ActionResult> RemoveItemFromCart(int userId, int productId, int amount)
    {
        var cart = await context.ShoppingCart.Include(shoppingCart => shoppingCart.Items).FirstOrDefaultAsync(c => c.UserId == userId);
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
        
        await context.SaveChangesAsync();
        return NoContent();
        
    }
    
    [HttpDelete("{userId}")]
    public async Task<ActionResult<ShoppingCartDto>> ClearCart(int userId)
    {
        var cart = await context.ShoppingCart.Include(shoppingCart => shoppingCart.Items).FirstOrDefaultAsync(c => c.UserId == userId);
        if(cart == null) return NotFound();
        
        context.ShoppingCartItem.RemoveRange(cart.Items);
        await context.SaveChangesAsync();
        return NoContent();
    }
}