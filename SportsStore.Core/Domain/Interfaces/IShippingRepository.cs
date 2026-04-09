using SportsStore.Core.Domain.Entities;

namespace SportsStore.Core.Domain.Interfaces;

public interface IShippingRepository
{
    Task<ShipmentRecord> CreateShipmentRecordAsync(ShipmentRecord record);
    Task<ShipmentRecord?> GetShipmentRecordByOrderIdAsync(int orderId);
    Task SaveAsync(ShipmentRecord record);
}
