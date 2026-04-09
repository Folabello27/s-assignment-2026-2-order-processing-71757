namespace SportsStore.Core.Domain.DTOs;

public class InventoryRecordDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public bool Success { get; set; }
    public string? Status { get; set; }
    public string? Message { get; set; }
    public string? StockAvailability { get; set; }
    public DateTime CheckedAt { get; set; }
}
