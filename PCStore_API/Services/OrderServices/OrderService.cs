using Microsoft.EntityFrameworkCore;
using PCStore_API.ApiResponse;
using PCStore_API.Data;
using PCStore_API.Extensions;
using PCStore_API.Models.Order;
using PCStore_API.Services.ShoppingCartServices;
using PCStore_Shared.Models.Order;

namespace PCStore_API.Services.OrderServices;

public class OrderService(PcStoreDbContext context, IShoppingCartService shoppingCartService, ILogger<OrderService> logger) : IOrderService
{
    public async Task<OrderDto> CreateOrder(int userId)
    {
        //Gets the user cart
        var cart = await context.ShoppingCart
            .Include(shoppingCart => shoppingCart.Items)
            .ThenInclude(shoppingCartItem => shoppingCartItem.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);
        
        if(cart == null)
            throw new NotFoundException("User has no cart");
        
        //Checks if the stock is enough
        foreach (var item in cart.Items.Where(item => item.Quantity > item.Product.ProductStock))
            throw new ValidationException($"Product {item.ProductId} has only {item.Product.ProductStock} in stock.");

        await using var transactionAsync = await context.Database.BeginTransactionAsync();
        try
        {
            //Updates the stock
            foreach (var item in cart.Items) item.Product.ProductStock -= item.Quantity;

            //Creates the order items
            var cartItems = cart.Items.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                ProductName = item.Product.ProductName,
                ProductPrice = item.Product.ProductPrice,
                Quantity = item.Quantity
            }).ToList();

            //Creates the order
            var createOrder = new Order
            {
                UserId = userId,
                Items = cartItems,
                OrderDate = DateTime.UtcNow,
                OrderDateUpdated = DateTime.UtcNow,
                OrderStatus = "Ordered",
                OrderTotal = cart.Items.Sum(i => i.Quantity * i.Product.ProductPrice)
            };

            //Removes the cart items and clears the cart
            context.Orders.Add(createOrder);
            await shoppingCartService.ClearShoppingCartAsync(userId);

            //Saves the changes
            await context.SaveChangesAsync();
            //Commits the transaction
            await transactionAsync.CommitAsync();
            logger.LogInformation("User {UserId} placed order {OrderId}", userId, createOrder.OrderId);
            
            return createOrder.ToDto();
        }
        catch
        {
            await transactionAsync.RollbackAsync();
            throw;
        }
    }

    public async Task<OrderDto> GetOrderByIdAsync(int userId, int orderId)
    {
        //Gets the user orders
        var findOrder = await context.Orders
            .Where(i => i.OrderId == orderId)
            .Include(o => o.Items)
            .FirstOrDefaultAsync(i => i.UserId == userId);

        //Checks if the user has orders
        if (findOrder == null)
            throw new NotFoundException("User has no orders");

        return findOrder.ToDto();

    }

    public async Task<List<OrderDto>> GetOrdersByUserIdAsync(int userId)
    {
        //Gets the user orders
        var findOrder = await context.Orders
            .Where(i => i.UserId == userId)
            .Include(o => o.Items)
            .ToListAsync();

        //Checks if the user has orders
        if (findOrder == null)
            throw new NotFoundException("User has no orders");

        return findOrder.Select(i => i.ToDto()).ToList();
    }

    public async Task<OrderDto> RefundOrder(int userId, int orderId)
    {
        //Gets the user orders
        var findOrder = await context.Orders
            .Where(i => i.UserId == userId)
            .Include(o => o.Items)
            .FirstOrDefaultAsync(i => i.OrderId == orderId);

        //Checks if the user has orders
        if (findOrder == null)
            throw new NotFoundException("User has no orders");

        await using var transactionAsync = await context.Database.BeginTransactionAsync();
        try
        {
            foreach (var item in findOrder.Items)
            {
                context.Products.Find(item.ProductId).ProductStock += item.Quantity;
            }
            
            //Saves the changes
            await context.SaveChangesAsync();
            //Commits the transaction
            await transactionAsync.CommitAsync();
            logger.LogInformation("Items have been added back to stock");
            
        }
        catch (Exception ex)
        {
            
        }

        return null;
    }
}