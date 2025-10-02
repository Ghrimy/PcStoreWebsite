using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCStore_API.ApiResponse;
using PCStore_API.Extensions;
using PCStore_API.Services.ShoppingCartServices;
using PCStore_Shared.Models.ShoppingCart;

namespace PCStore_API.Controllers;

[ApiController]
[Authorize(Roles = "User,Admin")]
[Route("api/[controller]")]
public class ShoppingCartController(IShoppingCartService shoppingCartService) : ControllerBase
{
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId)) return null;

        return userId;
    }

    [HttpGet]
    public async Task<ActionResult<ShoppingCartDto?>> GetCart()
    {
        //Gets userId and checks if it is valid
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<ShoppingCartDto>.FailureResponse("User not found"));

        //Gets the cart and converts it to the front end Dto
        var cart = await shoppingCartService.GetShoppingCartAsync(userId.Value);
        return Ok(ApiResponse<ShoppingCartDto>.SuccessResponse(cart.ToDto(), "Found cart"));
    }

    [HttpPost("items")]
    public async Task<ActionResult<ShoppingCartDto>> AddItemToCart([FromBody] [Required] ShoppingCartAddDto item)
    {
        //Gets userId and checks if it is valid
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<ShoppingCartDto>.FailureResponse("User not found"));

        //Adds the item to the cart and returns the cart
        //Converts cart to Dto and returns it

        var cart = await shoppingCartService.AddToShoppingCartAsync(userId.Value, item);
        return Ok(ApiResponse<ShoppingCartDto>.SuccessResponse(cart, "Item added successfully"));
    }

    [HttpPut("items")]
    public async Task<ActionResult<ShoppingCartDto>> UpdateItemsInCart(
        [FromBody] [Required] List<ShoppingCartUpdateDto> items)
    {
        //Gets userId and checks if it is valid
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<ShoppingCartDto>.FailureResponse("User not found"));

        var cart = await shoppingCartService.UpdateShoppingCartAsync(userId.Value, items);
        return Ok(ApiResponse<ShoppingCartDto>.SuccessResponse(cart, "Items updated successfully"));
    }

    [HttpDelete("items")]
    public async Task<ActionResult> RemoveItemFromCart([FromBody] [Required] List<ShoppingCartRemoveDto> items)
    {
        //Gets userId and checks if it is valid
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<ShoppingCartDto>.FailureResponse("User not found"));

        var cart = await shoppingCartService.RemoveFromShoppingCartAsync(userId.Value, items);
        return Ok(ApiResponse<ShoppingCartDto>.SuccessResponse(cart, "Item removed successfully"));
    }

    [HttpDelete]
    public async Task<ActionResult<ShoppingCartDto>> ClearCart()
    {
        //Gets userId and checks if it is valid
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<ShoppingCartDto>.FailureResponse("User not found"));

        var cart = await shoppingCartService.ClearShoppingCartAsync(userId.Value);
        return Ok(ApiResponse<ShoppingCartDto>.SuccessResponse(cart, "Cart cleared successfully"));
    }
}