namespace SportsStore.Core.Messaging.Contracts;

public record ShippingRequested
{
    public int OrderId { get; init; }
    public string? CustomerId { get; init; }
    public string CorrelationId { get; init; } = string.Empty;
    public ShippingAddress ShippingAddress { get; init; } = new();
    public List<OrderItemMessage> Items { get; init; } = new();
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public record ShippingAddress
{
    public string Name { get; init; } = string.Empty;
    public string Line1 { get; init; } = string.Empty;
    public string? Line2 { get; init; }
    public string? Line3 { get; init; }
    public string City { get; init; } = string.Empty;
    public string? State { get; init; }
    public string? Zip { get; init; }
    public string Country { get; init; } = string.Empty;
}
