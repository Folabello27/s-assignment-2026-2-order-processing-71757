namespace SportsStore.Core.Messaging.Contracts;

public record InventoryCheckFailed
{
    public int OrderId { get; init; }
    public string CorrelationId { get; init; } = string.Empty;
    public string Reason { get; init; } = "Insufficient stock";
    public DateTime CheckedAt { get; init; } = DateTime.UtcNow;
}
