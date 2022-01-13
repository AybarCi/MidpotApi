using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingWeb.CacheService.Interface
{
    public interface ICache
    {
        object Get(string key);
        T Get<T>(string key);
        void Set(string key, object data, TimeSpan? date = null);
        void Remove(string key);
    }
}
