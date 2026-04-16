namespace SportsStore.Core.Messaging.Contracts;

public record OrderSubmitted
{
    public int OrderId { get; init; }
    public string? CustomerId { get; init; }
    public string CorrelationId { get; init; } = string.Empty;
    public List<OrderItemMessage> Items { get; init; } = new();
    public decimal TotalAmount { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
