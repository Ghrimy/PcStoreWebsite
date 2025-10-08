using Microsoft.EntityFrameworkCore;
using PCStore_API.ApiResponse;
using PCStore_API.Data;
using PCStore_API.Extensions;
using PCStore_API.Models.Order;
using PCStore_API.Services.ShoppingCartServices;
using PCStore_Shared.Models.Order;
using DbUpdateConcurrencyException = Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;

namespace PCStore_API.Services.OrderServices;


public static class OrderStatuses
{
    public const string Ordered = "Ordered";
    public const string Refunded = "Refunded";
    public const string PartiallyRefunded = "PartiallyRefunded";
    public const string Processing = "Processing";
    public const string Cancelled = "Cancelled";
    public const string Shipped = "Shipped";
    public const string Completed = "Completed";
}


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
                OrderStatus = OrderStatuses.Ordered,
                OrderTotal = shoppingCartService.CalculateTotal(cart)
            };

            //Removes the cart items and clears the cart
            context.Orders.Add(createOrder);
            await shoppingCartService.ClearShoppingCartAsync(userId);

            //Saves the changes
            try
            {
                
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                logger.LogWarning("Stock conflict detected, rolling back transaction...");
                await transactionAsync.RollbackAsync();
                throw new ValidationException("One or more products were just updated by another customer. Please refresh your cart.");
            }
            
                            
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
        if (findOrder.Count == 0)
            throw new NotFoundException("User has no orders");

        return findOrder.Select(i => i.ToDto()).ToList();
    }

    public async Task<OrderDto> RefundOrder(int userId, int orderId, List<RefundItemDto> refundItems)
    {
        //Gets the user order
        var findOrder = await context.Orders
            .Where(i => i.UserId == userId)
            .Include(o => o.Items)
            .FirstOrDefaultAsync(i => i.OrderId == orderId);

        //Checks if the user has orders or if it has been ordered
        if (findOrder == null) throw new NotFoundException("User has no orders");
        if (findOrder.OrderStatus != "Ordered") throw new ValidationException("Order is not ordered");
        if (findOrder.OrderStatus == "Refunded") throw new ValidationException("Order has already been refunded");
        if (refundItems == null || refundItems.Count == 0)
            throw new ValidationException("No refund items provided.");


        await using var transactionAsync = await context.Database.BeginTransactionAsync();
        try
        {
            foreach (var item in findOrder.Items)
            {
                //Finds the product to refund and the quantity to refund
                var refundItem = refundItems.FirstOrDefault(i => i.ProductId == item.ProductId);
                if (refundItem == null) continue;
                
                //Checks if the quantity to refund is less than the remaining quantity
                if (refundItem.Quantity > item.Quantity - item.RefundedQuantity)
                    throw new ValidationException($"Cannot refund more than remaining quantity for product {item.ProductId}.");
                
                //Finds the product
                var product = await context.Products.FindAsync(item.ProductId);
                if (product == null) throw new NotFoundException("Product not found");

                
                //Updates the stock
                product.ProductStock += refundItem.Quantity;
                item.RefundedQuantity += refundItem.Quantity;
                
                logger.LogInformation("Items {productId} with quantity {Quantity} have been added back to stock", refundItem.ProductId, refundItem.Quantity);
            }

            findOrder.OrderStatus = findOrder.Items.All(i => i.RefundedQuantity == i.Quantity)
                ? OrderStatuses.Refunded
                : OrderStatuses.PartiallyRefunded;
            
            findOrder.OrderDateUpdated = DateTime.UtcNow;
            
            //Saves the changes
            //Commits the transaction
            await context.SaveChangesAsync();
            await transactionAsync.CommitAsync();
            logger.LogInformation("Order {OrderId} has been refunded", findOrder.OrderId);

        }
        catch (Exception ex)
        {
            await transactionAsync.RollbackAsync();
            logger.LogError(ex, "Error while refunding order");
            throw;
        }

        return findOrder.ToDto();
    }
}