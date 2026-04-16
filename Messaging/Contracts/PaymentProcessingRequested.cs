namespace SportsStore.Core.Messaging.Contracts;

public record PaymentProcessingRequested
{
    public int OrderId { get; init; }
    public string? CustomerId { get; init; }
    public string CorrelationId { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string? PaymentMethod { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
