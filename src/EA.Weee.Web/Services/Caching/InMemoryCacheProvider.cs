namespace EA.Weee.Web.Services.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Caching;
    using System.Threading.Tasks;
    using System.Web;

    /// <summary>
    /// This provider uses the caching classes from the .NET framework's
    /// System.Runtime.Caching namespace. This provides an in-memory cache 
    /// linked to the current AppDomain.
    /// </summary>
    public class InMemoryCacheProvider : ICacheProvider
    {
        private readonly MemoryCache memoryCache = MemoryCache.Default;

        public Task Add(string cache, string key, object value, TimeSpan duration)
        {
            if (value != null)
            {
                string fullKey = GetFullKey(cache, key);

                CacheItemPolicy policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = DateTime.UtcNow + duration;

                memoryCache.Add(fullKey, value, policy);
            }
            return Task.FromResult<object>(null);
        }

        public Task<CacheResult<T>> Fetch<T>(string cache, string key)
        {
            string fullKey = GetFullKey(cache, key);

            CacheItem item = memoryCache.GetCacheItem(fullKey);

            CacheResult<T> result;

            if (item != null)
            {
                if (typeof(T).IsAssignableFrom(item.Value.GetType()))
                {
                    result = CacheResult<T>.Found((T)item.Value);
                }
                else
                {
                    result = CacheResult<T>.NotFound();
                }
            }
            else
            {
                result = CacheResult<T>.NotFound();
            }

            return Task.FromResult(result);
        }

        private static string GetFullKey(string cacheName, string key)
        {
            return string.Format("WEEE:{0}:{1}", cacheName, key);
        }

        public Task Remove(string cache, string key)
        {
            string fullKey = GetFullKey(cache, key);

            memoryCache.Remove(fullKey);

            return Task.FromResult<object>(null);
        }
    }
}