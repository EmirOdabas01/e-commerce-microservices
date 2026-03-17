using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Catalog.API.Behaviors;

public interface ICacheable
{
    string CacheKey { get; }
    TimeSpan CacheDuration => TimeSpan.FromMinutes(5);
}

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheable
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(IDistributedCache cache, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var cacheKey = request.CacheKey;

        try
        {
            var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);

            if (cached is not null)
            {
                _logger.LogInformation("Cache hit for {CacheKey}", cacheKey);
                return JsonSerializer.Deserialize<TResponse>(cached)!;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis cache read failed for {CacheKey}, falling through to handler", cacheKey);
        }

        var response = await next();

        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = request.CacheDuration
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(response), options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis cache write failed for {CacheKey}", cacheKey);
        }

        return response;
    }
}
