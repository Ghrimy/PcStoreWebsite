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
            UserId = order.UserId,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
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
        return new OrderItemDto
        {
            ProductId = orderItem.ProductId,
            Quantity = orderItem.Quantity
        };
    }


    public static OrderRefundHistoryDto ToDto(this OrderRefundHistory orderRefundHistory)
    {
        return new OrderRefundHistoryDto()
        {
            OrderId = orderRefundHistory.OrderId,
            Date = orderRefundHistory.Date,
            Reason = orderRefundHistory.Reason,
            RefundAmount = orderRefundHistory.RefundAmount,
            RefundedItems = orderRefundHistory.RefundedItems.Select(i => new OrderRefundItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ProductPrice = i.ProductPrice,
                Quantity = i.Quantity
            }).ToList(),
        };

    }

    public static OrderRefundItemDto ToDto(this OrderRefundItem orderRefundItem)
    {
        return new OrderRefundItemDto()
        {
            ProductId = orderRefundItem.ProductId,
            ProductName = orderRefundItem.ProductName,
            ProductPrice = orderRefundItem.ProductPrice,
            Quantity = orderRefundItem.Quantity
        };
    }
}