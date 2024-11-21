using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace BlogApi.Api.Services.Interfaces;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(30);
    public CacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }
    public bool Exists(string key)
    {
        return _memoryCache.TryGetValue(key, out _);
    }

    public T? Get<T>(string key)
    {
        if (_memoryCache.TryGetValue(key, out T value)) return value;
        return default;
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }

    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        expiration ??= _defaultExpiration;
        _memoryCache.Set(key, value, expiration.Value);

    }
}
