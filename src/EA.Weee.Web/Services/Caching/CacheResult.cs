namespace EA.Weee.Web.Services.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Represents the result of an attempt to retrieve an item from a cache.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CacheResult<T>
    {
        /// <summary>
        /// Indicates the result of the attempt to find the item.
        /// </summary>
        public bool FoundInCache { get; private set; }
        
        /// <summary>
        /// If the item was found, Value will provide the item.
        /// Otherwise, Value will be default(T).
        /// </summary>
        public T Value { get; private set; }

        private CacheResult(bool foundInCache, T value)
        {
            FoundInCache = foundInCache;
            Value = value;
        }

        public static CacheResult<T> NotFound()
        {
            return new CacheResult<T>(false, default(T));
        }

        public static CacheResult<T> Found(T value)
        {
            return new CacheResult<T>(true, value);
        }
    }
}