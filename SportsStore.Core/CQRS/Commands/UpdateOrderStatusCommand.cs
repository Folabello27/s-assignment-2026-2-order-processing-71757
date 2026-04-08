using MediatR;

namespace SportsStore.Core.CQRS.Commands;

public record UpdateOrderStatusCommand(
    int OrderId,
    string Status,
    string? Details = null) : IRequest<bool>;
