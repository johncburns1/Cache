using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace HACache.Clients
{
    public interface ClientBase
    {
        public Task<CacheEntry> GetAsync(object key);
        public Task AddAsync(object key, CacheEntry value);
        public Task<bool> ExistsAsync(object key);
        public Task RemoveAsync(object key);
    }
}
