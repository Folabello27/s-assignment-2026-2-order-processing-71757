namespace SportsStore.Core.Messaging.Contracts;

public record InventoryCheckCompleted
{
    public int OrderId { get; init; }
    public string CorrelationId { get; init; } = string.Empty;
    public string Message { get; init; } = "Inventory confirmed successfully";
    public Dictionary<int, int> ReservedStock { get; init; } = new();
    public Dictionary<int, int> StockAvailability { get; init; } = new();
    public DateTime CheckedAt { get; init; } = DateTime.UtcNow;
}
