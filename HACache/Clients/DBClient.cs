using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HACache.Clients
{
    public class DBClient : ClientBase
    {
        private Dictionary<object, object> mockDB = new Dictionary<object, object>();

        public DBClient()
        {
        }

        public async Task AddAsync(object key, CacheEntry value)
        {
            await Task.Delay(0);
            mockDB[key] = value;
        }

        public async Task<bool> ExistsAsync(object key)
        {
            return await Task.FromResult<bool>(mockDB.ContainsKey(key));
        }

        public async Task<CacheEntry> GetAsync(object key)
        {
            return await Task.FromResult<CacheEntry>(ConvertToCacheEntry(mockDB.GetValueOrDefault(key, null)));
        }

        public async Task RemoveAsync(object key)
        {
            await Task.FromResult(mockDB.Remove(key));
        }

        public async Task<long> GetUniqueCacheKeyAsync()
        {
            return await Task.FromResult<long>(LongRandom());
        }

        public CacheEntry ConvertToCacheEntry(object dbEntry)
        {
            return (CacheEntry)dbEntry;
        }

        private long LongRandom()
        {
            var random    = new Random();
            long longRand = (long)random.Next();

            return longRand;
        }
    }
}
