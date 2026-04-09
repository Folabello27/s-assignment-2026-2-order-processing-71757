using MediatR;

namespace SportsStore.Core.CQRS.Commands;

public record ProcessInventoryResultCommand(
    int OrderId,
    bool Success,
    string? Message = null,
    Dictionary<int, int>? StockAvailability = null) : IRequest<bool>;
