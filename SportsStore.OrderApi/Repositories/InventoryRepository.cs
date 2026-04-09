using SportsStore.Core.Domain.Entities;
using SportsStore.Core.Domain.Interfaces;
using SportsStore.OrderApi.Data;

namespace SportsStore.OrderApi.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly OrderDbContext _context;

    public InventoryRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryRecord> CreateInventoryRecordAsync(InventoryRecord record)
    {
        _context.InventoryRecords.Add(record);
        await _context.SaveChangesAsync();
        return record;
    }

    public async Task<InventoryRecord?> GetInventoryRecordByOrderIdAsync(int orderId)
    {
        return await Task.FromResult(_context.InventoryRecords.FirstOrDefault(i => i.OrderId == orderId));
    }

    public async Task SaveAsync(InventoryRecord record)
    {
        var existing = await _context.InventoryRecords.FindAsync(record.Id);
        if (existing != null)
        {
            _context.Entry(existing).CurrentValues.SetValues(record);
        }
        else
        {
            _context.InventoryRecords.Add(record);
        }
        await _context.SaveChangesAsync();
    }
}
