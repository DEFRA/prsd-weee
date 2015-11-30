namespace EA.Weee.Web.Services.Caching
{
    using System;
    using System.Threading.Tasks;

    public class SingleItemCache<TValue>
    {
        private readonly ICacheProvider provider;
        private readonly Func<Task<TValue>> fetchFromApi;

        public string CacheName { get; private set; }
        public TimeSpan Duration { get; private set; }

        public SingleItemCache(
            ICacheProvider provider,
            string cacheName,
            TimeSpan duration,
            Func<Task<TValue>> fetchFromApi)
        {
            this.provider = provider;
            this.fetchFromApi = fetchFromApi;

            CacheName = cacheName;
            Duration = duration;
        }

        public async Task<TValue> Fetch()
        {
            CacheResult<TValue> resultFromCache = await provider.Fetch<TValue>(CacheName, "Key");
            if (resultFromCache.FoundInCache)
            {
                return resultFromCache.Value;
            }
            else
            {
                TValue value;

                value = await fetchFromApi();

                await provider.Add(CacheName, "Key", value, Duration);

                return value;
            }
        }

        public async Task InvalidateCache()
        {
            await provider.Remove(CacheName, "Key");
        }
    }
}