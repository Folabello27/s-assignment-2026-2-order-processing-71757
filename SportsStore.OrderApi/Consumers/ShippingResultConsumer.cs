using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SportsStore.Core.Domain.Entities;
using SportsStore.Core.Domain.Interfaces;
using SportsStore.Core.Messaging.Contracts;
using SportsStore.Infrastructure.Messaging;

namespace SportsStore.OrderApi.Consumers;

public class ShippingResultConsumer : RabbitMqConsumer<ShippingCreated>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ShippingResultConsumer(RabbitMqConnection connection, IServiceScopeFactory scopeFactory)
        : base(connection, "orderapi-shipping-created", "ShippingCreated")
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task HandleMessageAsync(ShippingCreated message, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var shippingRepository = scope.ServiceProvider.GetRequiredService<IShippingRepository>();
        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

        Log.Information("OrderApi: Recording shipment for Order {OrderId}", message.OrderId);

        var shipment = new ShipmentRecord
        {
            OrderId = message.OrderId,
            Status = "Created",
            TrackingNumber = message.TrackingNumber,
            Carrier = message.Carrier,
            ServiceType = "Standard",
            EstimatedDeliveryDate = message.EstimatedDeliveryDate,
            CreatedAt = message.Timestamp
        };

        await shippingRepository.SaveAsync(shipment);
        await orderRepository.UpdateOrderStatusAsync(message.OrderId, "ShippingCreated");
    }
}

public class OrderCompletedConsumer : RabbitMqConsumer<OrderCompleted>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public OrderCompletedConsumer(RabbitMqConnection connection, IServiceScopeFactory scopeFactory)
        : base(connection, "orderapi-order-completed", "OrderCompleted")
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task HandleMessageAsync(OrderCompleted message, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

        Log.Information("OrderApi: Marking Order {OrderId} as completed", message.OrderId);

        await orderRepository.UpdateOrderStatusAsync(message.OrderId, "Completed");
    }
}
