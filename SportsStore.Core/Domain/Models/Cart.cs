using SportsStore.Core.Domain.Entities;

namespace SportsStore.Core.Domain.Models;

public class Cart
{
    public List<CartLine> Lines { get; set; } = new();

    public void AddItem(Product product, int quantity = 1)
    {
        var line = Lines.FirstOrDefault(l => l.Product?.ProductID == product.ProductID);
        
        if (line == null)
        {
            Lines.Add(new CartLine
            {
                Product = product,
                Quantity = quantity
            });
        }
        else
        {
            line.Quantity += quantity;
        }
    }

    public void RemoveLine(Product product)
    {
        Lines.RemoveAll(l => l.Product?.ProductID == product.ProductID);
    }

    public decimal ComputeTotalValue()
    {
        return Lines.Sum(l => l.Product?.Price * l.Quantity ?? 0);
    }

    public void Clear()
    {
        Lines.Clear();
    }
}

public class CartLine
{
    public int CartLineID { get; set; }
    public Product? Product { get; set; }
    public int Quantity { get; set; }
}
