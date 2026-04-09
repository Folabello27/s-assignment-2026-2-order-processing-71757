using SportsStore.Core.Domain.Entities;
using SportsStore.Core.Domain.Interfaces;
using SportsStore.OrderApi.Data;

namespace SportsStore.OrderApi.Repositories;

public class ShippingRepository : IShippingRepository
{
    private readonly OrderDbContext _context;

    public ShippingRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<ShipmentRecord> CreateShipmentRecordAsync(ShipmentRecord record)
    {
        _context.ShipmentRecords.Add(record);
        await _context.SaveChangesAsync();
        return record;
    }

    public async Task<ShipmentRecord?> GetShipmentRecordByOrderIdAsync(int orderId)
    {
        return await Task.FromResult(_context.ShipmentRecords.FirstOrDefault(s => s.OrderId == orderId));
    }

    public async Task SaveAsync(ShipmentRecord record)
    {
        var existing = await _context.ShipmentRecords.FindAsync(record.Id);
        if (existing != null)
        {
            _context.Entry(existing).CurrentValues.SetValues(record);
        }
        else
        {
            _context.ShipmentRecords.Add(record);
        }
        await _context.SaveChangesAsync();
    }
}
