namespace SportsStore.Core.Messaging.Contracts;

public record InventoryCheckRequested
{
    public int OrderId { get; init; }
    public string CorrelationId { get; init; } = string.Empty;
    public List<OrderItemMessage> Items { get; init; } = new();
}
