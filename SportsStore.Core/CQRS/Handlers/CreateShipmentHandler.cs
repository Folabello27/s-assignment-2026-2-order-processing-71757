using MediatR;
using Serilog;
using SportsStore.Core.CQRS.Commands;
using SportsStore.Core.Domain.Entities;
using SportsStore.Core.Domain.Interfaces;
using SportsStore.Core.Messaging.Contracts;

namespace SportsStore.Core.CQRS.Handlers;

public class CreateShipmentHandler : IRequestHandler<CreateShipmentCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IShippingRepository _shippingRepository;
    private readonly IEventPublisher _eventPublisher;

    public CreateShipmentHandler(
        IOrderRepository orderRepository,
        IShippingRepository shippingRepository,
        IEventPublisher eventPublisher)
    {
        _orderRepository = orderRepository;
        _shippingRepository = shippingRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<bool> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderRepository.GetOrderByIdAsync(request.OrderId);
            if (order == null)
            {
                Log.Error("Order {OrderId} not found for shipment creation", request.OrderId);
                return false;
            }

            // Generate tracking number
            var trackingNumber = $"SS{DateTime.UtcNow:yyyyMMdd}{request.OrderId:D6}";
            var estimatedDelivery = DateTime.UtcNow.AddDays(3).Date;

            // Save shipment record
            var shipmentRecord = new ShipmentRecord
            {
                OrderId = request.OrderId,
                Status = "Created",
                TrackingNumber = trackingNumber,
                Carrier = request.Carrier ?? "FedEx",
                ServiceType = request.ServiceType ?? "Standard",
                EstimatedDeliveryDate = estimatedDelivery,
                CreatedAt = DateTime.UtcNow
            };
            await _shippingRepository.SaveAsync(shipmentRecord);

            await _orderRepository.UpdateOrderStatusAsync(request.OrderId, "Completed");

            // Publish shipping created event
            var shippingCreated = new ShippingCreated
            {
                OrderId = order.OrderID,
                CustomerId = order.CustomerId,
                CorrelationId = order.CorrelationId ?? Guid.NewGuid().ToString(),
                TrackingNumber = trackingNumber,
                Carrier = shipmentRecord.Carrier,
                EstimatedDeliveryDate = estimatedDelivery,
                Timestamp = DateTime.UtcNow
            };

            await _eventPublisher.PublishAsync(shippingCreated, "ShippingCreated");

            // Publish order completed event
            var orderCompleted = new OrderCompleted
            {
                OrderId = order.OrderID,
                CustomerId = order.CustomerId,
                CorrelationId = order.CorrelationId ?? Guid.NewGuid().ToString(),
                TotalAmount = order.Items.Sum(i => i.Quantity * i.UnitPrice),
                CompletedAt = DateTime.UtcNow
            };

            await _eventPublisher.PublishAsync(orderCompleted, "OrderCompleted");

            Log.Information("Order {OrderId} completed. Tracking: {TrackingNumber}",
                order.OrderID, trackingNumber);

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating shipment for order {OrderId}", request.OrderId);
            return false;
        }
    }
}
