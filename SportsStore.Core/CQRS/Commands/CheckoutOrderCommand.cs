using MediatR;
using SportsStore.Core.Domain.DTOs;

namespace SportsStore.Core.CQRS.Commands;

public record CheckoutOrderCommand : IRequest<OrderResponseDto>
{
    public string? CustomerId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Line1 { get; init; } = string.Empty;
    public string? Line2 { get; init; }
    public string? Line3 { get; init; }
    public string City { get; init; } = string.Empty;
    public string? State { get; init; }
    public string? Zip { get; init; }
    public string Country { get; init; } = string.Empty;
    public bool GiftWrap { get; init; }
    public List<OrderItemDto> Items { get; init; } = new();
    public string? PaymentMethod { get; init; }
    public string? CardNumber { get; init; }
}
