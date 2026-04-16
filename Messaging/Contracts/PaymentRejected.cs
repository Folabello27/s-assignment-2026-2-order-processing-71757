namespace SportsStore.Core.Messaging.Contracts;

public record PaymentRejected
{
    public int OrderId { get; init; }
    public string CorrelationId { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateTime RejectedAt { get; init; } = DateTime.UtcNow;
}
