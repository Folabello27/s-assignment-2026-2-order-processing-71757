namespace SportsStore.Core.Messaging.Contracts;

public record PaymentApproved
{
    public int OrderId { get; init; }
    public string CorrelationId { get; init; } = string.Empty;
    public string TransactionId { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Message { get; init; } = "Payment approved";
    public DateTime ApprovedAt { get; init; } = DateTime.UtcNow;
}
