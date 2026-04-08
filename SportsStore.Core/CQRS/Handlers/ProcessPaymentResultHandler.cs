using MediatR;
using Serilog;
using SportsStore.Core.CQRS.Commands;
using SportsStore.Core.Domain.Entities;
using SportsStore.Core.Domain.Interfaces;
using SportsStore.Core.Messaging.Contracts;

namespace SportsStore.Core.CQRS.Handlers;

public class ProcessPaymentResultHandler : IRequestHandler<ProcessPaymentResultCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IEventPublisher _eventPublisher;

    public ProcessPaymentResultHandler(
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository,
        IEventPublisher eventPublisher)
    {
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<bool> Handle(ProcessPaymentResultCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderRepository.GetOrderByIdAsync(request.OrderId);
            if (order == null)
            {
                Log.Error("Order {OrderId} not found for payment result processing", request.OrderId);
                return false;
            }

            // Save payment record
            var paymentRecord = new PaymentRecord
            {
                OrderId = request.OrderId,
                Status = request.Success ? "Approved" : "Rejected",
                TransactionId = request.TransactionId,
                Amount = order.Items.Sum(i => i.Quantity * i.UnitPrice),
                ProcessedAt = DateTime.UtcNow,
                Message = request.Message
            };
            await _paymentRepository.SaveAsync(paymentRecord);

            if (request.Success)
            {
                await _orderRepository.UpdateOrderStatusAsync(request.OrderId, "ShippingPending");

                // Publish shipping request
                var shippingRequest = new ShippingRequested
                {
                    OrderId = order.OrderID,
                    CustomerId = order.CustomerId,
                    CorrelationId = order.CorrelationId ?? Guid.NewGuid().ToString(),
                    ShippingAddress = new ShippingAddress
                    {
                        Name = order.Name,
                        Line1 = order.Line1,
                        Line2 = order.Line2,
                        Line3 = order.Line3,
                        City = order.City,
                        State = order.State,
                        Zip = order.Zip,
                        Country = order.Country
                    },
                    Items = order.Items.Select(i => new OrderItemMessage
                    {
                        ProductId = (int)i.ProductID,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice
                    }).ToList(),
                    Timestamp = DateTime.UtcNow
                };

                await _eventPublisher.PublishAsync(shippingRequest, "ShippingRequested");

                Log.Information("Published ShippingRequested for order {OrderId}", order.OrderID);
            }
            else
            {
                await _orderRepository.UpdateOrderStatusAsync(request.OrderId, "Failed");

                // Publish order failed event
                var orderFailed = new OrderFailed
                {
                    OrderId = order.OrderID,
                    CorrelationId = order.CorrelationId ?? Guid.NewGuid().ToString(),
                    Reason = $"Payment failed: {request.Message}",
                    FailedAt = DateTime.UtcNow
                };

                await _eventPublisher.PublishAsync(orderFailed, "OrderFailed");

                Log.Warning("Order {OrderId} failed due to payment failure: {Message}",
                    order.OrderID, request.Message);
            }

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error processing payment result for order {OrderId}", request.OrderId);
            return false;
        }
    }
}
