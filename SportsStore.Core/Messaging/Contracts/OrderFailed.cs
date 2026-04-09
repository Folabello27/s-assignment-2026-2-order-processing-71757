namespace SportsStore.Core.Messaging.Contracts;

public record OrderFailed
{
    public int OrderId { get; init; }
    public string CorrelationId { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
    public string FailedStage { get; init; } = string.Empty;
    public DateTime FailedAt { get; init; } = DateTime.UtcNow;
}
