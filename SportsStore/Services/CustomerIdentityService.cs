using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace SportsStore.Services;

public class CustomerIdentityService
{
    private const string StorageKey = "sportsstore.customer-id";
    private readonly ProtectedLocalStorage _storage;

    public CustomerIdentityService(ProtectedLocalStorage storage)
    {
        _storage = storage;
    }

    public async Task<string> GetCustomerIdAsync()
    {
        var storedValue = await _storage.GetAsync<string>(StorageKey);
        if (storedValue.Success && !string.IsNullOrWhiteSpace(storedValue.Value))
        {
            return storedValue.Value;
        }

        var customerId = Guid.NewGuid().ToString("N");
        await _storage.SetAsync(StorageKey, customerId);
        return customerId;
    }
}
