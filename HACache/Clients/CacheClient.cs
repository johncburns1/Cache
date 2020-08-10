using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

using Microsoft.Extensions.Configuration;

using HACache.Models;
using System.Threading.Tasks;
using System.Transactions;

namespace HACache.Clients
{
    public class CacheClient : ClientBase
    {
        private Dictionary<long, HACache> mockCaches = new Dictionary<long, HACache>();
        private long replicaKey;

        public CacheClient()
        {
            Task.Run(async () => this.replicaKey = await GetReplicaCacheKeyAsync()).Wait();
        }

        public long ReplicaKey { get; set; }

        public async Task<bool> AddAsync(object key, CacheEntry value)
        {
            return await mockCaches[replicaKey].Add(key, value) != null;
        }

        public async Task<bool> ExistsAsync(object key)
        {
            return await mockCaches[replicaKey].Exists(key);
        }

        public async Task<CacheEntry> GetAsync(object key)
        {
            return (CacheEntry) await mockCaches[replicaKey].Get(key);
        }

        public async Task RemoveAsync(object key)
        {
            await Task.FromResult(mockCaches[replicaKey].Remove(key));
        }

        public void AddCache(HACache cache)
        {
            mockCaches[cache.CacheKey] = cache;
        }

        public async Task<long> GetReplicaCacheKeyAsync()
        {
            foreach (var cache in mockCaches.Values)
            {
                if (cache.AvailableForReplica)
                {
                    cache.AvailableForReplica = false;
                    return await Task.FromResult<long>(cache.CacheKey);
                }
            }

            return await Task.FromResult<long>(-1L);
        }
    }
}
