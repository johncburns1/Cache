using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using HACache.Models;

namespace HACache.Clients
{
    public class CacheClient : ClientBase
    {
        private Dictionary<long, HACache> mockCaches = new Dictionary<long, HACache>();
        private HashSet<long> replicaKeys;

        public CacheClient()
        {
        }

        public long ReplicaKey { get; set; }

        public async Task AddAsync(object key, CacheEntry value)
        {
            foreach (var worker in mockCaches.Values)
            {
                await mockCaches[worker.CacheKey].Add(key, value);
            }
        }

        public async Task<bool> ExistsAsync(object key)
        {
            return await mockCaches[mockCaches.First().Value.CacheKey].Exists(key);
        }

        public async Task<CacheEntry> GetAsync(object key)
        {
            return (CacheEntry) await mockCaches[mockCaches.First().Value.CacheKey].Get(key);
        }

        public async Task RemoveAsync(object key)
        {
            foreach (var worker in mockCaches.Values)
            {
                await Task.FromResult(mockCaches[worker.CacheKey].Remove(key));
            }
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
