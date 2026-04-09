using System.Net.Http.Json;

namespace SportsStore.Services;

// Local DTOs for the client app
public class OrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class OrderResponseDto
{
    public int OrderId { get; set; }
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
    public string OrderStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public decimal TotalAmount { get; set; }
    public int ItemCount { get; set; }
}

public class ProductDto
{
    public int ProductID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
}

public class CartService
{
    private readonly HttpClient _httpClient;
    private readonly List<CartItem> _items = new();

    public event Action? OnChange;

    public CartService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("OrderApi");
    }

    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();
    public int ItemCount => _items.Sum(i => i.Quantity);
    public decimal Total => _items.Sum(i => i.UnitPrice * i.Quantity);

    public void AddItem(ProductDto product, int quantity)
    {
        var existingItem = _items.FirstOrDefault(i => i.ProductId == product.ProductID);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            _items.Add(new CartItem
            {
                ProductId = product.ProductID,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = quantity
            });
        }
        NotifyStateChanged();
    }

    public void RemoveItem(int productId)
    {
        _items.RemoveAll(i => i.ProductId == productId);
        NotifyStateChanged();
    }

    public void UpdateQuantity(int productId, int quantity)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            if (quantity <= 0)
            {
                _items.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }
            NotifyStateChanged();
        }
    }

    public void Clear()
    {
        _items.Clear();
        NotifyStateChanged();
    }

    public async Task<OrderResponseDto?> CheckoutAsync(CheckoutRequest checkoutRequest)
    {
        var command = new CheckoutOrderCommand
        {
            CustomerId = checkoutRequest.CustomerId ?? Guid.NewGuid().ToString(),
            Name = checkoutRequest.Name,
            Line1 = checkoutRequest.Line1,
            Line2 = checkoutRequest.Line2,
            Line3 = checkoutRequest.Line3,
            City = checkoutRequest.City,
            State = checkoutRequest.State,
            Zip = checkoutRequest.Zip,
            Country = checkoutRequest.Country,
            GiftWrap = checkoutRequest.GiftWrap,
            Items = _items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        var response = await _httpClient.PostAsJsonAsync("api/orders/checkout", command);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<OrderResponseDto>();
            Clear();
            return result;
        }
        return null;
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}

public class CartItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}

public class CheckoutRequest
{
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
}

public class CheckoutOrderCommand
{
    public string? CustomerId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Line1 { get; init; } = string.Empty;
    public string? Line2 { get; init; }
    public string? Line3 { get; init; }
    public string City { get; init; } = string.Empty;
    public string? State { get; init; }
    public string? Zip { get; init; }
    public string Country { get; init; } = string.Empty;
    public bool GiftWrap { get; init; }
    public List<OrderItemDto> Items { get; init; } = new();
}
