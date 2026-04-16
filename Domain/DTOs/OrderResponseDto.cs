namespace SportsStore.Core.Domain.DTOs;

public class OrderResponseDto
{
    [System.Text.Json.Serialization.JsonPropertyName("orderId")]
    public int OrderID { get; set; }
    public decimal TotalAmount { get; set; }
    public int ItemCount { get; set; }
    public string? CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Line1 { get; set; } = string.Empty;
    public string? Line2 { get; set; }
    public string? Line3 { get; set; }
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string? Zip { get; set; }
    public string Country { get; set; } = string.Empty;
    public bool GiftWrap { get; set; }
    public bool Shipped { get; set; }
    public string OrderStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
    public InventoryRecordDto? InventoryRecord { get; set; }
    public PaymentRecordDto? PaymentRecord { get; set; }
    public ShipmentRecordDto? ShipmentRecord { get; set; }
}
