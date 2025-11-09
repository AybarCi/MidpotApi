using DatingWeb.CacheService.Interface;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace DatingWeb.CacheService
{
    public class RedisCache : IRedisCache
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<RedisCache> _logger;

        public RedisCache(IDistributedCache distributedCache, ILogger<RedisCache> logger)
        {
            _distributedCache = distributedCache;
            _logger = logger;
        }

        public string Name { get; set; } = "RedisCache";

        public object Get(string key)
        {
            throw new NotSupportedException("Use GetAsync for Redis cache");
        }

        public T Get<T>(string key)
        {
            throw new NotSupportedException("Use GetAsync<T> for Redis cache");
        }

        public void Set(string key, object data, TimeSpan? date = null)
        {
            throw new NotSupportedException("Use SetAsync for Redis cache");
        }

        public void Remove(string key)
        {
            throw new NotSupportedException("Use RemoveAsync for Redis cache");
        }

        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                var value = await _distributedCache.GetStringAsync(key);
                if (string.IsNullOrEmpty(value))
                {
                    return default;
                }
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting cache key: {key}");
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            try
            {
                var options = new DistributedCacheEntryOptions();
                if (expiry.HasValue)
                {
                    options.SetAbsoluteExpiration(expiry.Value);
                }
                else
                {
                    options.SetAbsoluteExpiration(TimeSpan.FromHours(1));
                }

                var serializedValue = JsonConvert.SerializeObject(value);
                await _distributedCache.SetStringAsync(key, serializedValue, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error setting cache key: {key}");
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _distributedCache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing cache key: {key}");
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                var value = await _distributedCache.GetStringAsync(key);
                return !string.IsNullOrEmpty(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking cache key existence: {key}");
                return false;
            }
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null)
        {
            try
            {
                var value = await GetAsync<T>(key);
                if (value != null && !value.Equals(default(T)))
                {
                    return value;
                }

                var newValue = await factory();
                if (newValue != null)
                {
                    await SetAsync(key, newValue, expiry);
                }
                return newValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetOrSetAsync for key: {key}");
                return await factory();
            }
        }

        public async Task RefreshAsync(string key)
        {
            try
            {
                await _distributedCache.RefreshAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error refreshing cache key: {key}");
            }
        }

        public async Task RemovePatternAsync(string pattern)
        {
            // Redis'te pattern ile silme işlemi için özel implementasyon gerekli
            // Bu basit implementasyonda sadece exact key'leri silebiliyoruz
            // Gerçek implementasyonda Redis ConnectionMultiplexer kullanarak pattern-based silme yapılabilir

            // Şimdilik sadece exact key'leri silebiliyoruz
            // TODO: Redis ConnectionMultiplexer ile pattern-based silme implemente et
            await Task.CompletedTask;
        }
    }
}