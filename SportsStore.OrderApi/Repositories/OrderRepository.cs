using Microsoft.EntityFrameworkCore;
using SportsStore.Core.Domain.Entities;
using SportsStore.Core.Domain.Interfaces;
using SportsStore.OrderApi.Data;

namespace SportsStore.OrderApi.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context)
    {
        _context = context;
    }

    public IQueryable<Order> Orders => _context.Orders
        .Include(o => o.Items)
        .ThenInclude(i => i.Product)
        .Include(o => o.InventoryRecord)
        .Include(o => o.PaymentRecord)
        .Include(o => o.ShipmentRecord)
        .AsQueryable();

    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await Orders
            .FirstOrDefaultAsync(o => o.OrderID == id);
    }

    public async Task<List<Order>> GetOrdersAsync(int page = 1, int pageSize = 20, string? status = null)
    {
        var query = Orders;

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(o => o.OrderStatus == status);
        }

        return await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<Order>> GetCustomerOrdersAsync(string customerId)
    {
        return await Orders
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetOrderCountAsync(string? status = null)
    {
        if (string.IsNullOrEmpty(status))
        {
            return await _context.Orders.CountAsync();
        }
        return await _context.Orders.CountAsync(o => o.OrderStatus == status);
    }

    public async Task SaveOrderAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateOrderStatusAsync(int orderId, string status)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.OrderStatus = status;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
