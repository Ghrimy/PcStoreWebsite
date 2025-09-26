using PCStore_API.Models.Order;
using PCStore_Shared.Models.Order;

namespace PCStore_API.Extensions;

public static class OrderExtensions
{
    // order to dto
    public static OrderDto ToDto(this Order order)
    {
        return new OrderDto
        {
            OrderId = order.OrderId,
            UserId = order.UserId,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ProductPrice = i.ProductPrice,
                Quantity = i.Quantity
            }).ToList(),
            OrderTotal = order.OrderTotal,
            OrderStatus = order.OrderStatus,
            OrderDate = order.OrderDate,
            OrderDateUpdated = order.OrderDateUpdated
        };
    }

    // order item to dto
    public static OrderItemDto ToDto(this OrderItem orderItem)
    {
        return new OrderItemDto()
        {
            ProductId = orderItem.ProductId,
            ProductName = orderItem.ProductName,
            ProductPrice = orderItem.ProductPrice,
            Quantity = orderItem.Quantity,
        };
    }
}