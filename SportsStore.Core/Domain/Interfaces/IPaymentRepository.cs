using SportsStore.Core.Domain.Entities;

namespace SportsStore.Core.Domain.Interfaces;

public interface IPaymentRepository
{
    Task<PaymentRecord> CreatePaymentRecordAsync(PaymentRecord record);
    Task<PaymentRecord?> GetPaymentRecordByOrderIdAsync(int orderId);
    Task SaveAsync(PaymentRecord record);
}
