using System.ComponentModel.DataAnnotations;
using SportsStore.Core.Domain.Entities;

namespace SportsStore.Core.Domain.Entities;

public class Order
{
    [Key]
    public int OrderID { get; set; }
    
    public string? CustomerId { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string Line1 { get; set; } = string.Empty;
    
    public string? Line2 { get; set; }
    
    public string? Line3 { get; set; }
    
    [Required]
    public string City { get; set; } = string.Empty;
    
    public string? State { get; set; }
    
    public string? Zip { get; set; }
    
    [Required]
    public string Country { get; set; } = string.Empty;
    
    public bool GiftWrap { get; set; }
    public bool Shipped { get; set; }
    
    // Order tracking
    public string OrderStatus { get; set; } = "Cart";
    public string? CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public InventoryRecord? InventoryRecord { get; set; }
    public PaymentRecord? PaymentRecord { get; set; }
    public ShipmentRecord? ShipmentRecord { get; set; }
}
