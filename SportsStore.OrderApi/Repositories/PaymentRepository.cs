using SportsStore.Core.Domain.Entities;
using SportsStore.Core.Domain.Interfaces;
using SportsStore.OrderApi.Data;

namespace SportsStore.OrderApi.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly OrderDbContext _context;

    public PaymentRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentRecord> CreatePaymentRecordAsync(PaymentRecord record)
    {
        _context.PaymentRecords.Add(record);
        await _context.SaveChangesAsync();
        return record;
    }

    public async Task<PaymentRecord?> GetPaymentRecordByOrderIdAsync(int orderId)
    {
        return await Task.FromResult(_context.PaymentRecords.FirstOrDefault(p => p.OrderId == orderId));
    }

    public async Task SaveAsync(PaymentRecord record)
    {
        var existing = await _context.PaymentRecords.FindAsync(record.Id);
        if (existing != null)
        {
            _context.Entry(existing).CurrentValues.SetValues(record);
        }
        else
        {
            _context.PaymentRecords.Add(record);
        }
        await _context.SaveChangesAsync();
    }
}
