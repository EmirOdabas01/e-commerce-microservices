using System.Text.Json;
using Basket.API.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.API.Data;

public class BasketRepository : IBasketRepository
{
    private readonly IDistributedCache _cache;

    public BasketRepository(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<ShoppingCart?> GetBasket(string userName, CancellationToken cancellationToken = default)
    {
        var basket = await _cache.GetStringAsync(userName, cancellationToken);
        return string.IsNullOrEmpty(basket) ? null : JsonSerializer.Deserialize<ShoppingCart>(basket);
    }

    public async Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellationToken = default)
    {
        await _cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket), cancellationToken);
        return basket;
    }

    public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(userName, cancellationToken);
        return true;
    }
}
