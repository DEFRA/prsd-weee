namespace EA.Weee.Web.Services.Caching
{
    using System;
    using System.Threading.Tasks;

    public interface ICacheProvider
    {
        /// <summary>
        /// Adds the object to the cache, replacing any existing object with the same key.
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        Task Add(string cache, string key, object value, TimeSpan duration);
        
        /// <summary>
        /// Fetches an object from the cache, casting it as the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<CacheResult<T>> Fetch<T>(string cache, string key);

        /// <summary>
        /// Removes an item from the cache.
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Task Remove(string cache, string key);
    }
}
