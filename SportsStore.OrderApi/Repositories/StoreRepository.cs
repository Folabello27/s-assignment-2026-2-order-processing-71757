using Microsoft.EntityFrameworkCore;
using SportsStore.Core.Domain.Entities;
using SportsStore.Core.Domain.Interfaces;
using SportsStore.OrderApi.Data;

namespace SportsStore.OrderApi.Repositories;

public class StoreRepository : IStoreRepository
{
    private readonly OrderDbContext _context;

    public StoreRepository(OrderDbContext context)
    {
        _context = context;
    }

    public IQueryable<Product> Products => _context.Products.AsQueryable();

    public async Task<Product?> GetProductByIdAsync(long id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<List<Product>> GetProductsAsync(int page = 1, int pageSize = 10, string? category = null)
    {
        var query = _context.Products.AsQueryable();
        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(p => p.Category == category);
        }
        return await query
            .OrderBy(p => p.ProductID)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetProductCountAsync(string? category = null)
    {
        if (string.IsNullOrEmpty(category))
            return await _context.Products.CountAsync();
        return await _context.Products.CountAsync(p => p.Category == category);
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        return await _context.Products.Select(p => p.Category).Distinct().ToListAsync();
    }

    public async Task SaveProductAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task CreateProductAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(Product product)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> CheckInventoryAsync(long productId, int requiredQuantity)
    {
        var product = await _context.Products.FindAsync(productId);
        return product != null && product.StockQuantity >= requiredQuantity;
    }

    public async Task<bool> ReserveInventoryAsync(long productId, int quantity)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product != null && product.StockQuantity >= quantity)
        {
            product.StockQuantity -= quantity;
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }
}
