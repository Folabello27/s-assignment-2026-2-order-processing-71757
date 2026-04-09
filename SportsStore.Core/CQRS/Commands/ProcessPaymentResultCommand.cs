using MediatR;

namespace SportsStore.Core.CQRS.Commands;

public record ProcessPaymentResultCommand(
    int OrderId,
    bool Success,
    string? TransactionId = null,
    string? Message = null) : IRequest<bool>;
