using System.ComponentModel.DataAnnotations;
using SportsStore.Core.Domain.Entities;

namespace SportsStore.Core.Domain.Entities;

public class OrderItem
{
    [Key]
    public int OrderItemID { get; set; }
    
    public int OrderID { get; set; }
    public Order? Order { get; set; }
    
    public int ProductID { get; set; }
    public Product? Product { get; set; }
    
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
