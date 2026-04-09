using SportsStore.Core.Domain.Entities;

namespace SportsStore.Core.Domain.Interfaces;

public interface IInventoryRepository
{
    Task<InventoryRecord> CreateInventoryRecordAsync(InventoryRecord record);
    Task<InventoryRecord?> GetInventoryRecordByOrderIdAsync(int orderId);
    Task SaveAsync(InventoryRecord record);
}
