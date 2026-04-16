using MediatR;
using SportsStore.Core.CQRS.Queries;
using SportsStore.Core.Domain.Interfaces;

namespace SportsStore.Core.CQRS.Handlers;

public class GetDashboardSummaryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryResponse>
{
    private readonly IOrderRepository _orderRepository;

    public GetDashboardSummaryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<DashboardSummaryResponse> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var totalOrders = await _orderRepository.GetOrderCountAsync();
        var pendingOrders = await _orderRepository.GetOrderCountAsync("PaymentPending") +
                           await _orderRepository.GetOrderCountAsync("ShippingPending") +
                           await _orderRepository.GetOrderCountAsync("Submitted");
        var completedOrders = await _orderRepository.GetOrderCountAsync("Completed");
        var failedOrders = await _orderRepository.GetOrderCountAsync("Failed");

        var orders = await _orderRepository.GetOrdersAsync(1, int.MaxValue);
        var totalRevenue = orders
            .Where(o => o.OrderStatus == "Completed")
            .Sum(o => o.Items.Sum(i => i.Quantity * i.UnitPrice));

        var ordersByStatus = orders
            .GroupBy(o => o.OrderStatus)
            .ToDictionary(g => g.Key, g => g.Count());

        return new DashboardSummaryResponse(
            totalOrders,
            pendingOrders,
            completedOrders,
            failedOrders,
            totalRevenue,
            ordersByStatus);
    }
}
