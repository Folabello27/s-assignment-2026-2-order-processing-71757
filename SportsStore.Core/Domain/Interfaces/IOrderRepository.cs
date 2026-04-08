using SportsStore.Core.Domain.Entities;

namespace SportsStore.Core.Domain.Interfaces;

public interface IOrderRepository
{
    IQueryable<Order> Orders { get; }
    Task<Order?> GetOrderByIdAsync(int id);
    Task<List<Order>> GetOrdersAsync(int page = 1, int pageSize = 20, string? status = null);
    Task<List<Order>> GetCustomerOrdersAsync(string customerId);
    Task<int> GetOrderCountAsync(string? status = null);
    Task SaveOrderAsync(Order order);
    Task UpdateOrderStatusAsync(int orderId, string status);
}
