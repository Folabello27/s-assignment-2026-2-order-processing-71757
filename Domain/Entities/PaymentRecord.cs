using System.ComponentModel.DataAnnotations;

namespace SportsStore.Core.Domain.Entities;

public class PaymentRecord
{
    [Key]
    public int Id { get; set; }
    
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    
    public string? TransactionId { get; set; }
    public string? Status { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public decimal Amount { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}
