namespace SportsStore.Core.Messaging.Contracts;

public record ShippingCreated
{
    public int OrderId { get; init; }
    public string? CustomerId { get; init; }
    public string CorrelationId { get; init; } = string.Empty;
    public string TrackingNumber { get; init; } = Guid.NewGuid().ToString();
    public string Carrier { get; init; } = "Standard Shipping";
    public DateTime EstimatedDispatchDate { get; init; } = DateTime.UtcNow.AddDays(2);
    public DateTime EstimatedDeliveryDate { get; init; } = DateTime.UtcNow.AddDays(5);
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
