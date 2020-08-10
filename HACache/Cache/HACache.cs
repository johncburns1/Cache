using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using HACache.Clients;
using HACache.Models;

namespace HACache
{
    public class HACache : CacheBase
    {
        protected readonly DBClient    dbClient;
        protected readonly CacheClient cacheClient;

        private long cacheKey;

        public HACache(
            DBClient       dbClient,
            CacheClient    cacheClient,
            EvictionPolicy evictionPolicy)
            :base(evictionPolicy)
        {
            this.dbClient    = dbClient;
            this.cacheClient = cacheClient;

            Task.Run(async () => this.cacheKey = await dbClient.GetUniqueCacheKeyAsync());
        }

        public long CacheKey { get; set; }
        public bool AvailableForReplica { get; set; }

        public override async Task<object> Add(object key, object value)
        {
            await semaphore.WaitAsync();

            try
            {
                if (!IsInitialized())
                {
                    return null;
                }

                if (value.GetType() == typeof(CacheEntry))
                {
                    await ProcessCacheEntryAsync(key, (CacheEntry)value);
                }
                else
                {
                    await ProcessNonCacheEntryAsync(key, value);
                }

                return key;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public override async Task<bool> Exists(object key)
        {
            await semaphore.WaitAsync();

            try
            {
                if (!IsInitialized())
                {
                    return false;
                }

                return await cacheClient.ExistsAsync(key);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public override async Task<object> Get(object key)
        {
            await semaphore.WaitAsync();

            try
            {
                if (!IsInitialized())
                {
                    return null;
                }

                return await cacheClient.GetAsync(key);
            }
            finally
            {
                semaphore.Release();
            }
        }

        private async Task ProcessCacheEntryAsync(object key, CacheEntry entry)
        {
            // add new entry locally

            queue.AddFirst(key);
            entries[key] = entry;

            // try and add new value to the backing db.

            await dbClient.AddAsync(key, entry);
        }

        private async Task ProcessNonCacheEntryAsync(object key, object value)
        {
            if (!entries.ContainsKey(key))
            {
                if (queue.Count == capacity)
                {
                    // evict if we are at capacity

                    var evicted = evictionPolicy.Evict(queue);
                    entries.Remove(evicted);

                    // remove from workers

                    await cacheClient.RemoveAsync(evicted);
                }
            }
            else
            {
                // handle duplicates

                queue.Remove(key);
                await cacheClient.RemoveAsync(key);
            }

            // add new entry locally

            queue.AddFirst(key);
            entries[key] = new CacheEntry(value, cacheKey);

            // add the new entry to other caches
            // if we have a problem replicating to 
            // replica cache write to backing db until
            // replica cache is back up.

            if (!await cacheClient.AddAsync(key, entries[key]))
            {
                await dbClient.AddAsync(key, entries[key]);
            }
        }
    }
}
