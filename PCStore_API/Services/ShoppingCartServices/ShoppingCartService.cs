using Microsoft.EntityFrameworkCore;
using PCStore_API.ApiResponse;
using PCStore_API.Data;
using PCStore_API.Extensions;
using PCStore_API.Models.ShoppingCart;
using PCStore_Shared.Models.ShoppingCart;

namespace PCStore_API.Services.ShoppingCartServices;

public class ShoppingCartService(PcStoreDbContext context, ILogger<ShoppingCartService> logger) : IShoppingCartService
{
    public async Task<ShoppingCart> GetShoppingCartAsync(int userId)
    {
        //Finds the cart for the user
        var cart = await context.ShoppingCart
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        //Creates a new cart if the user doesn't have one
        if (cart == null)
        {
            var createCart = new ShoppingCart
            {
                UserId = userId,
                Items = new List<ShoppingCartItem>(),
                LastUpdated = DateTime.UtcNow
            };

            context.ShoppingCart.Add(createCart);
            await context.SaveChangesAsync();
            logger.LogInformation("Created new cart for user");
            return cart;
        }

        //Clears the cart if it's been inactive for 30 days
        if (cart.LastUpdated < DateTime.UtcNow.AddDays(-30))
        {
            cart.Items.Clear();
            logger.LogInformation("User {UserId} cart {CartId} was cleared due to inactivity", userId,
                cart.ShoppingCartId);
            await context.SaveChangesAsync();
        }

        //Returns the cart
        return cart;
    }

    public async Task<ShoppingCartDto> AddToShoppingCartAsync(int userId, ShoppingCartAddDto item)
    {
        //Checks if the quantity added is valid
        if (item.Quantity <= 0) throw new ValidationException("Quantity must be greater than zero.");

        //Gets the cart
        var cart = await GetShoppingCartAsync(userId);

        //Finds the product exists
        var product = await context.Products.FindAsync(item.ProductId) ??
                      throw new NotFoundException($"Product {item.ProductId} not found.");
        ;

        //Checks if the product exists in the cart
        var existingItemInCart = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existingItemInCart == null)
        {
            //Checks if the product is in stock if now throws error
            if (item.Quantity > product.ProductStock)
                throw new ValidationException($"Product {item.ProductId} has only {product.ProductStock} in stock.");

            //Adds the product to the cart
            cart.Items.Add(new ShoppingCartItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            });
        }
        else
        {
            //Checks if base quantity plus added quantity exceeds stock
            if (existingItemInCart.Quantity + item.Quantity > product.ProductStock)
                throw new ValidationException("Quantity exceeds available stock.");

            //Updates the quantity if it doesn't exceed stock
            existingItemInCart.Quantity += item.Quantity;
            logger.LogInformation("Product {ProductId} quantity updated to {Quantity}", item.ProductId,
                existingItemInCart.Quantity);
        }

        cart.LastUpdated = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return cart.ToDto();
    }

    public async Task<ShoppingCartDto> RemoveFromShoppingCartAsync(int userId, List<ShoppingCartRemoveDto> items)
    {
        //Gets the cart
        var cart = await GetShoppingCartAsync(userId);

        //Checks if any items are being removed
        if (items.Count == 0) throw new NotFoundException("No items to remove");

        //Loops through the items and checks if they are valid, removes them if they are
        foreach (var product in items)
        {
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == product.ProductId);
            if (existingItem == null)
                throw new NotFoundException($"Product {product.ProductId} not found.");

            //Checks if the quantity is valid, removes item from cart if it is or is more than the existing quantity
            if (product.Quantity >= existingItem.Quantity) cart.Items.Remove(existingItem);

            //Updates the quantity if it doesn't exceed stock'
            existingItem.Quantity -= product.Quantity;
        }

        //Updates the cart
        cart.LastUpdated = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return cart.ToDto();
    }


    public async Task<ShoppingCartDto> UpdateShoppingCartAsync(int userId, List<ShoppingCartUpdateDto> items)
    {
        //Gets the cart
        var cart = await GetShoppingCartAsync(userId);

        //Checks if any items are being updated
        if (items.Count == 0) throw new NotFoundException("No items to update");

        //Loops through the items and checks if they are valid, updates them if they are
        foreach (var product in items)
        {
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == product.ProductId);
            if (existingItem == null)
                throw new NotFoundException($"Product {product.ProductId} not found.");

            if (product.NewQuantity <= 0)
                throw new ValidationException(
                    $"Product {product.ProductId} with Quantity: {product.NewQuantity}  can't be/less zero.");

            if (product.NewQuantity > existingItem.Product.ProductStock)
                throw new ValidationException(
                    $"Product {product.ProductId} with Quantity: {product.NewQuantity}  can't be more than stock.");

            existingItem.Quantity = product.NewQuantity;
        }

        //Updates the cart
        cart.LastUpdated = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return cart.ToDto();
    }

    public async Task<ShoppingCartDto> ClearShoppingCartAsync(int userId)
    {
        //Gets the cart
        var cart = await GetShoppingCartAsync(userId);

        //Removes all items from the cart and clears it
        context.ShoppingCartItem.RemoveRange(cart.Items);
        cart.Items.Clear();

        //Updates the cart
        cart.LastUpdated = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return cart.ToDto();
    }
}