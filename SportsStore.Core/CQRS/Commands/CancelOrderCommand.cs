using MediatR;

namespace SportsStore.Core.CQRS.Commands;

public record CancelOrderCommand(int OrderId, string? Reason = null) : IRequest<bool>;
