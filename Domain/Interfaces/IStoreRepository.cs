using SportsStore.Core.Domain.Entities;

namespace SportsStore.Core.Domain.Interfaces;

public interface IStoreRepository
{
    IQueryable<Product> Products { get; }
    Task<Product?> GetProductByIdAsync(long id);
    Task<List<Product>> GetProductsAsync(int page = 1, int pageSize = 10, string? category = null);
    Task<int> GetProductCountAsync(string? category = null);
    Task<List<string>> GetCategoriesAsync();
    Task SaveProductAsync(Product product);
    Task CreateProductAsync(Product product);
    Task DeleteProductAsync(Product product);
    Task<bool> CheckInventoryAsync(long productId, int requiredQuantity);
    Task<bool> ReserveInventoryAsync(long productId, int quantity);
}
