namespace SportsStore.Core.Messaging.Contracts;

public record OrderCompleted
{
    public int OrderId { get; init; }
    public string? CustomerId { get; init; }
    public string CorrelationId { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public DateTime CompletedAt { get; init; } = DateTime.UtcNow;
}
