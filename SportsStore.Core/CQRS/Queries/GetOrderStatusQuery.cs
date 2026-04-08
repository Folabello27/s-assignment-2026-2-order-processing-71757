using MediatR;

namespace SportsStore.Core.CQRS.Queries;

public record GetOrderStatusQuery(int OrderId) : IRequest<OrderStatusResponse?>;

public record OrderStatusResponse(
    int OrderId,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string? InventoryStatus,
    string? PaymentStatus,
    string? ShippingStatus);
