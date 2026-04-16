using MediatR;

namespace SportsStore.Core.CQRS.Commands;

public record CreateShipmentCommand(
    int OrderId,
    string? Carrier = null,
    string? ServiceType = null) : IRequest<bool>;
