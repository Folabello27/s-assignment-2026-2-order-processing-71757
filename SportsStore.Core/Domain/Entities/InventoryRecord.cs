using System.ComponentModel.DataAnnotations;

namespace SportsStore.Core.Domain.Entities;

public class InventoryRecord
{
    [Key]
    public int Id { get; set; }
    
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    
    public bool Success { get; set; }
    public string? Status { get; set; }
    public string? Message { get; set; }
    public string? StockAvailability { get; set; }
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
}
