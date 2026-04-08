using System.ComponentModel.DataAnnotations;

namespace SportsStore.Core.Domain.Entities;

public class Product
{
    [Key]
    public int ProductID { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    
    [Required]
    public string Category { get; set; } = string.Empty;
    
    public int StockQuantity { get; set; } = 0;
}
