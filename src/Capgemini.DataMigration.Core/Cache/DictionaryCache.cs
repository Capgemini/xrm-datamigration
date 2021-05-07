using System;
using System.Runtime.Caching;

namespace Capgemini.DataMigration.Cache
{
    /// <summary>
    /// Base class for caches based on an in-memory dictionary.  This class does not perform any memory management, and uses a Dictionary for Memory efficiency rather than the System.Rumtime.Caching framework which has a large overhead per cache entry and is thus unsuitable for large volume, small payload caching applications.
    /// </summary>
    /// <typeparam name="TItem">The cache type.</typeparam>
    public abstract class DictionaryCache<TItem>
    {
        private System.Collections.Generic.Dictionary<string, TItem> cacheDictionary = new System.Collections.Generic.Dictionary<string, TItem>();

        protected TItem GetCachedItem(string cacheKey)
        {
            return this.cacheDictionary.TryGetValue(cacheKey, out TItem result) ? result : default(TItem);
        }

        protected void SetCachedItem(string cacheKey, TItem item)
        {
            // Store cached item
            lock (cacheDictionary)
            {
                if (item == null)
                {
                    cacheDictionary.Remove(cacheKey);
                }
                else
                {
                    cacheDictionary[cacheKey] = item;
                }
            }
        }
    }
}