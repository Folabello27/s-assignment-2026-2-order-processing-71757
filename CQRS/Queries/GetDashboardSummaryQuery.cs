using MediatR;

namespace SportsStore.Core.CQRS.Queries;

public record GetDashboardSummaryQuery : IRequest<DashboardSummaryResponse>;

public record DashboardSummaryResponse(
    int TotalOrders,
    int PendingOrders,
    int CompletedOrders,
    int FailedOrders,
    decimal TotalRevenue,
    Dictionary<string, int> OrdersByStatus);
