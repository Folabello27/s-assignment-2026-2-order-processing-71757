using System.Net.Http.Json;

namespace SportsStore.Services;

public class ProductService
{
    private readonly HttpClient _httpClient;

    public ProductService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("OrderApi");
    }

    public async Task<List<ProductDto>?> GetProductsAsync(string? category = null, int page = 1, int pageSize = 20)
    {
        var url = $"api/products?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrEmpty(category))
        {
            url += $"&category={category}";
        }
        return await _httpClient.GetFromJsonAsync<List<ProductDto>>(url);
    }

    public async Task<ProductDto?> GetProductAsync(int productId)
    {
        return await _httpClient.GetFromJsonAsync<ProductDto>($"api/products/{productId}");
    }

    public async Task<List<string>?> GetCategoriesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<string>>("api/products/categories");
    }
}
