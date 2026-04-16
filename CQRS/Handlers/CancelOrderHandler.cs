using MediatR;
using Serilog;
using SportsStore.Core.CQRS.Commands;
using SportsStore.Core.Domain.Interfaces;
using SportsStore.Core.Messaging.Contracts;

namespace SportsStore.Core.CQRS.Handlers;

public class CancelOrderHandler : IRequestHandler<CancelOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEventPublisher _eventPublisher;

    public CancelOrderHandler(IOrderRepository orderRepository, IEventPublisher eventPublisher)
    {
        _orderRepository = orderRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderRepository.GetOrderByIdAsync(request.OrderId);
            if (order == null)
            {
                Log.Error("Order {OrderId} not found for cancellation", request.OrderId);
                return false;
            }

            // Only allow cancellation if order is not already completed
            if (order.OrderStatus == "Completed")
            {
                Log.Warning("Order {OrderId} cannot be cancelled - already completed", request.OrderId);
                return false;
            }

            await _orderRepository.UpdateOrderStatusAsync(request.OrderId, "Cancelled");

            // Publish order failed event for cancellation
            var orderFailed = new OrderFailed
            {
                OrderId = order.OrderID,
                CorrelationId = order.CorrelationId ?? Guid.NewGuid().ToString(),
                Reason = $"Order cancelled: {request.Reason ?? "Customer request"}",
                FailedAt = DateTime.UtcNow
            };

            await _eventPublisher.PublishAsync(orderFailed, "OrderFailed");

            Log.Information("Order {OrderId} cancelled. Reason: {Reason}",
                request.OrderId, request.Reason ?? "Customer request");

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error cancelling order {OrderId}", request.OrderId);
            return false;
        }
    }
}
