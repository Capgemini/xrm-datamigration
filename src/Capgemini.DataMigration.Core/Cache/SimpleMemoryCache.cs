using System;
using System.Runtime.Caching;

namespace Capgemini.DataMigration.Cache
{
    public abstract class SimpleMemoryCache<TItem>
    {
        protected int ExpirationMinutes { get; set; } = 60;

        protected abstract TItem CreateCachedItem(string cacheKey);

        protected TItem GetCachedItem(string cacheKey)
        {
            var cachedObject = MemoryCache.Default.Get(cacheKey, null);

            if (cachedObject != null)
            {
                return (TItem)cachedObject;
            }

            // If not in cache, then needs to be populated
            lock (MemoryCache.Default)
            {
                // Check to see if anyone wrote to the cache in the meantime
                cachedObject = MemoryCache.Default.Get(cacheKey, null);

                if (cachedObject != null)
                {
                    return (TItem)cachedObject;
                }

                TItem newIstance = CreateCachedItem(cacheKey);

                CacheItemPolicy cip = new CacheItemPolicy
                {
                    AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(ExpirationMinutes))
                };

                MemoryCache.Default.Set(cacheKey, newIstance, cip);

                return newIstance;
            }
        }

        protected TItem TryGetCachedItem(string cacheKey)
        {
            var cachedObject = MemoryCache.Default.Get(cacheKey, null);
            return (TItem)cachedObject;
        }

        protected void SetCachedItem (string cacheKey, TItem item)
        {
            // Store cached item
            lock (MemoryCache.Default)
            {
                MemoryCache.Default.Set(cacheKey, item, new DateTimeOffset(DateTime.Now.AddMinutes(ExpirationMinutes)));
            }
        }
    }
}