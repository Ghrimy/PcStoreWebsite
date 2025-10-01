using PCStore_API.Models.ShoppingCart;
using PCStore_Shared.Models.ShoppingCart;

namespace PCStore_API.Extensions;

public static class ShoppingCartExtension
{
    public static ShoppingCartDto ToDto(this ShoppingCart shoppingCart)
    {
        return new ShoppingCartDto
        {
            ShoppingCartItems = shoppingCart.Items.Select(i => new ShoppingCartItemDto
            {
                ProductName = i.Product.ProductName,
                ProductId = i.ProductId,
                ProductPrice = i.Product.ProductPrice,
                Quantity = i.Quantity

            }).ToList(),
            LastUpdated = shoppingCart.LastUpdated,
        };

    }


    public static ShoppingCartItemDto ItemToDto(this ShoppingCartItem shoppingCartItem)
    {
        return new ShoppingCartItemDto
        {
            ProductId = shoppingCartItem.ProductId,
            ProductName = shoppingCartItem.Product.ProductName,
            ProductPrice = shoppingCartItem.Product.ProductPrice,
            Quantity = shoppingCartItem.Quantity
        };
    }

}