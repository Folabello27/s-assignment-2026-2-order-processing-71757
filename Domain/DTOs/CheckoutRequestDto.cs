using System.ComponentModel.DataAnnotations;

namespace SportsStore.Core.Domain.DTOs;

public class CheckoutRequestDto
{
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
    public string? CustomerId { get; set; }
    public string? PaymentMethod { get; set; }
}
