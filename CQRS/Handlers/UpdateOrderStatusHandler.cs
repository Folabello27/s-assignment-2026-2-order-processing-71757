using MediatR;
using Serilog;
using SportsStore.Core.CQRS.Commands;
using SportsStore.Core.Domain.Interfaces;

namespace SportsStore.Core.CQRS.Handlers;

public class UpdateOrderStatusHandler : IRequestHandler<UpdateOrderStatusCommand, bool>
{
    private readonly IOrderRepository _orderRepository;

    public UpdateOrderStatusHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<bool> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _orderRepository.UpdateOrderStatusAsync(request.OrderId, request.Status);

            Log.Information("Order {OrderId} status updated to {Status}. Details: {Details}",
                request.OrderId, request.Status, request.Details);

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to update order {OrderId} status to {Status}",
                request.OrderId, request.Status);
            return false;
        }
    }
}
