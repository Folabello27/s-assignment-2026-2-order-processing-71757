using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SportsStore.Core.CQRS.Commands;
using SportsStore.Core.Messaging.Contracts;
using SportsStore.Infrastructure.Messaging;

namespace SportsStore.OrderApi.Consumers;

public class PaymentResultConsumer : RabbitMqConsumer<PaymentApproved>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public PaymentResultConsumer(RabbitMqConnection connection, IServiceScopeFactory scopeFactory)
        : base(connection, "orderapi-payment-approved", "PaymentApproved")
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task HandleMessageAsync(PaymentApproved message, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        Log.Information("OrderApi: Processing payment success for Order {OrderId}", message.OrderId);

        await mediator.Send(
            new ProcessPaymentResultCommand(
                message.OrderId,
                true,
                message.TransactionId,
                "Payment approved"),
            cancellationToken);
    }
}

public class PaymentRejectedConsumer : RabbitMqConsumer<PaymentRejected>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public PaymentRejectedConsumer(RabbitMqConnection connection, IServiceScopeFactory scopeFactory)
        : base(connection, "orderapi-payment-rejected", "PaymentRejected")
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task HandleMessageAsync(PaymentRejected message, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        Log.Information("OrderApi: Processing payment failure for Order {OrderId}", message.OrderId);

        await mediator.Send(
            new ProcessPaymentResultCommand(
                message.OrderId,
                false,
                null,
                message.Reason),
            cancellationToken);
    }
}
