using MediatR;
using AutoMapper;
using Serilog;
using SportsStore.Core.CQRS.Commands;
using SportsStore.Core.Domain.DTOs;
using SportsStore.Core.Domain.Entities;
using SportsStore.Core.Domain.Interfaces;
using SportsStore.Core.Messaging.Contracts;

namespace SportsStore.Core.CQRS.Handlers;

public class CheckoutOrderHandler : IRequestHandler<CheckoutOrderCommand, OrderResponseDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly IMapper _mapper;

    public CheckoutOrderHandler(
        IOrderRepository orderRepository,
        IStoreRepository storeRepository,
        IEventPublisher eventPublisher,
        IMapper mapper)
    {
        _orderRepository = orderRepository;
        _storeRepository = storeRepository;
        _eventPublisher = eventPublisher;
        _mapper = mapper;
    }

    public async Task<OrderResponseDto> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid().ToString();

        Log.Information("Starting checkout process for customer {CustomerId} with CorrelationId {CorrelationId}",
            request.CustomerId, correlationId);

        // Validate products and calculate total
        var orderItems = new List<OrderItem>();
        decimal totalAmount = 0;

        foreach (var item in request.Items)
        {
            var product = await _storeRepository.GetProductByIdAsync(item.ProductID);
            if (product == null)
            {
                throw new InvalidOperationException($"Product {item.ProductID} not found");
            }

            orderItems.Add(new OrderItem
            {
                ProductID = item.ProductID,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });

            totalAmount += product.Price * item.Quantity;
        }

        // Create order
        var order = new Order
        {
            CustomerId = request.CustomerId,
            Name = request.Name,
            Line1 = request.Line1,
            Line2 = request.Line2,
            Line3 = request.Line3,
            City = request.City,
            State = request.State,
            Zip = request.Zip,
            Country = request.Country,
            GiftWrap = request.GiftWrap,
            OrderStatus = "Submitted",
            CorrelationId = correlationId,
            Items = orderItems,
            CreatedAt = DateTime.UtcNow
        };

        await _orderRepository.SaveOrderAsync(order);

        Log.Information("Order {OrderId} created with status {Status}",
            order.OrderID, order.OrderStatus);

        // Publish OrderSubmitted event
        var orderSubmitted = new OrderSubmitted
        {
            OrderId = order.OrderID,
            CustomerId = order.CustomerId,
            CorrelationId = correlationId,
            Items = request.Items.Select(i => new OrderItemMessage
            {
                ProductId = i.ProductID,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList(),
            TotalAmount = totalAmount
        };

        await _eventPublisher.PublishAsync(orderSubmitted, "OrderSubmitted");

        Log.Information("Published OrderSubmitted event for order {OrderId}", order.OrderID);

        return _mapper.Map<OrderResponseDto>(order);
    }
}
