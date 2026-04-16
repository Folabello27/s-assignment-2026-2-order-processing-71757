using MediatR;
using SportsStore.Core.CQRS.Queries;
using SportsStore.Core.Domain.Interfaces;

namespace SportsStore.Core.CQRS.Handlers;

public class GetOrderStatusHandler : IRequestHandler<GetOrderStatusQuery, OrderStatusResponse?>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderStatusHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderStatusResponse?> Handle(GetOrderStatusQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetOrderByIdAsync(request.OrderId);
        if (order == null) return null;

        return new OrderStatusResponse(
            order.OrderID,
            order.OrderStatus,
            order.CreatedAt,
            order.UpdatedAt,
            order.InventoryRecord?.Status,
            order.PaymentRecord?.Status,
            order.ShipmentRecord?.Status);
    }
}
