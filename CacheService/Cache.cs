using DatingWeb.CacheService.Interface;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace DatingWeb.CacheService
{
    public class Cache : ICache
    {
        private readonly IMemoryCache _memoryCache;
        public Cache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public string Name { get; set; }

        public object Get(string key)
        {
            return _memoryCache.Get(key);
        }
        public T Get<T>(string key)
        {
            return _memoryCache.Get<T>(key);
        }
        public void Set(string key, object data, TimeSpan? date = null)
        {
            _memoryCache.Set(key, data, date.HasValue ? date.Value : TimeSpan.FromHours(5));
        }
        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}
