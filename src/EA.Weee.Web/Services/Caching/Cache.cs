namespace EA.Weee.Web.Services.Caching
{
    using System;
    using System.Threading.Tasks;

    public class Cache<TKey, TValue>
    {
        private readonly ICacheProvider provider;
        private readonly Func<TKey, string> hashKey;
        private readonly Func<TKey, Task<TValue>> fetchFromApi;

        public string CacheName { get; private set; }
        public TimeSpan Duration { get; private set; }

        public Cache(
            ICacheProvider provider,
            string cacheName,
            TimeSpan duration,
            Func<TKey, string> hashKey,
            Func<TKey, Task<TValue>> fetchFromApi)
        {
            this.provider = provider;
            this.hashKey = hashKey;
            this.fetchFromApi = fetchFromApi;

            CacheName = cacheName;
            Duration = duration;
        }

        public async Task<TValue> Fetch(TKey key)
        {
            string hash = hashKey(key);

            CacheResult<TValue> resultFromCache = await provider.Fetch<TValue>(CacheName, hash);
            if (resultFromCache.FoundInCache)
            {
                return resultFromCache.Value;
            }
            else
            {
                TValue value;

                value = await fetchFromApi(key);

                await provider.Add(CacheName, hash, value, Duration);

                return value;
            }
        }
    }
}