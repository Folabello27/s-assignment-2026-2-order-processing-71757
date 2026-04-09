using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SportsStore.Core.CQRS.Commands;
using SportsStore.Core.Messaging.Contracts;
using SportsStore.Infrastructure.Messaging;

namespace SportsStore.OrderApi.Consumers;

public class InventoryResultConsumer : RabbitMqConsumer<InventoryCheckCompleted>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public InventoryResultConsumer(RabbitMqConnection connection, IServiceScopeFactory scopeFactory)
        : base(connection, "orderapi-inventory-completed", "InventoryCheckCompleted")
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task HandleMessageAsync(InventoryCheckCompleted message, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        Log.Information("OrderApi: Processing inventory success for Order {OrderId}", message.OrderId);

        await mediator.Send(
            new ProcessInventoryResultCommand(
                message.OrderId,
                true,
                "Inventory confirmed",
                message.StockAvailability),
            cancellationToken);
    }
}

public class InventoryFailedConsumer : RabbitMqConsumer<InventoryCheckFailed>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public InventoryFailedConsumer(RabbitMqConnection connection, IServiceScopeFactory scopeFactory)
        : base(connection, "orderapi-inventory-failed", "InventoryCheckFailed")
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task HandleMessageAsync(InventoryCheckFailed message, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        Log.Information("OrderApi: Processing inventory failure for Order {OrderId}", message.OrderId);

        await mediator.Send(
            new ProcessInventoryResultCommand(
                message.OrderId,
                false,
                message.Reason),
            cancellationToken);
    }
}
