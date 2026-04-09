using System.ComponentModel.DataAnnotations;

namespace SportsStore.Core.Domain.Entities;

public class ShipmentRecord
{
    [Key]
    public int Id { get; set; }
    
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    
    public string? TrackingNumber { get; set; }
    public string? Carrier { get; set; }
    public string? ServiceType { get; set; }
    public DateTime? EstimatedDispatchDate { get; set; }
    public DateTime? EstimatedDeliveryDate { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? ShippedAt { get; set; }
    public string Status { get; set; } = "Pending";
}
