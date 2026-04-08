namespace SportsStore.Core.Domain.DTOs;

public class ShipmentRecordDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string? TrackingNumber { get; set; }
    public string? Carrier { get; set; }
    public DateTime? EstimatedDispatchDate { get; set; }
    public DateTime? ShippedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}
