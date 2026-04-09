using System.Net.Http.Json;

namespace SportsStore.Services;

public class OrderService
{
    private readonly HttpClient _httpClient;

    public OrderService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("OrderApi");
    }

    public async Task<PaginatedOrdersResponse?> GetOrdersAsync(int page = 1, int pageSize = 20, string? status = null)
    {
        var url = $"api/orders?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrEmpty(status))
        {
            url += $"&status={status}";
        }
        return await _httpClient.GetFromJsonAsync<PaginatedOrdersResponse>(url);
    }

    public async Task<OrderResponseDto?> GetOrderAsync(int orderId)
    {
        return await _httpClient.GetFromJsonAsync<OrderResponseDto>($"api/orders/{orderId}");
    }

    public async Task<OrderStatusResponse?> GetOrderStatusAsync(int orderId)
    {
        return await _httpClient.GetFromJsonAsync<OrderStatusResponse>($"api/orders/{orderId}/status");
    }

    public async Task<List<OrderResponseDto>?> GetCustomerOrdersAsync(string customerId)
    {
        return await _httpClient.GetFromJsonAsync<List<OrderResponseDto>>($"api/customers/{customerId}/orders");
    }
}

public class PaginatedOrdersResponse
{
    public List<OrderResponseDto> Orders { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class OrderStatusResponse
{
    public int OrderId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? InventoryStatus { get; set; }
    public string? PaymentStatus { get; set; }
    public string? ShippingStatus { get; set; }
}
