using MediatR;
using Serilog;
using SportsStore.Core.CQRS.Commands;
using SportsStore.Core.Domain.Entities;
using SportsStore.Core.Domain.Interfaces;
using SportsStore.Core.Messaging.Contracts;

namespace SportsStore.Core.CQRS.Handlers;

public class ProcessInventoryResultHandler : IRequestHandler<ProcessInventoryResultCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IEventPublisher _eventPublisher;

    public ProcessInventoryResultHandler(
        IOrderRepository orderRepository,
        IInventoryRepository inventoryRepository,
        IEventPublisher eventPublisher)
    {
        _orderRepository = orderRepository;
        _inventoryRepository = inventoryRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<bool> Handle(ProcessInventoryResultCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderRepository.GetOrderByIdAsync(request.OrderId);
            if (order == null)
            {
                Log.Error("Order {OrderId} not found for inventory result processing", request.OrderId);
                return false;
            }

            // Save inventory record
            var inventoryRecord = new InventoryRecord
            {
                OrderId = request.OrderId,
                Status = request.Success ? "Confirmed" : "Failed",
                CheckedAt = DateTime.UtcNow,
                Message = request.Message,
                StockAvailability = request.StockAvailability != null
                    ? string.Join(",", request.StockAvailability.Select(kvp => $"{kvp.Key}:{kvp.Value}"))
                    : null
            };
            await _inventoryRepository.SaveAsync(inventoryRecord);

            if (request.Success)
            {
                await _orderRepository.UpdateOrderStatusAsync(request.OrderId, "PaymentPending");

                // Publish payment processing request
                var paymentRequest = new PaymentProcessingRequested
                {
                    OrderId = order.OrderID,
                    CustomerId = order.CustomerId,
                    CorrelationId = order.CorrelationId ?? Guid.NewGuid().ToString(),
                    Amount = order.Items.Sum(i => i.Quantity * i.UnitPrice),
                    Timestamp = DateTime.UtcNow
                };

                await _eventPublisher.PublishAsync(paymentRequest, "PaymentProcessingRequested");

                Log.Information("Published PaymentProcessingRequested for order {OrderId}", order.OrderID);
            }
            else
            {
                await _orderRepository.UpdateOrderStatusAsync(request.OrderId, "Failed");

                // Publish order failed event
                var orderFailed = new OrderFailed
                {
                    OrderId = order.OrderID,
                    CorrelationId = order.CorrelationId ?? Guid.NewGuid().ToString(),
                    Reason = $"Inventory check failed: {request.Message}",
                    FailedAt = DateTime.UtcNow
                };

                await _eventPublisher.PublishAsync(orderFailed, "OrderFailed");

                Log.Warning("Order {OrderId} failed due to inventory check failure: {Message}",
                    order.OrderID, request.Message);
            }

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error processing inventory result for order {OrderId}", request.OrderId);
            return false;
        }
    }
}
