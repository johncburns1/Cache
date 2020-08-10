using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HACache.Models;

namespace HACache
{
    public abstract class CacheBase
    {
        protected readonly EvictionPolicy evictionPolicy;
        protected readonly SemaphoreSlim  semaphore = new SemaphoreSlim(1, 1);

        protected LinkedList<object>             queue;
        protected Dictionary<object, CacheEntry> entries;

        protected int capacity;

        public CacheBase(EvictionPolicy evictionPolicy)
        {
            this.evictionPolicy = evictionPolicy;
        }

        public void Create(int size)
        {
            this.capacity = size;
            this.queue    = new LinkedList<object>();
            this.entries  = new Dictionary<object, CacheEntry>(capacity);
        }

        public abstract Task<object> Add(object key, object value);

        public abstract Task<object> Get(object key);

        public abstract Task<bool> Exists(object key);

        public async Task Remove(object key)
        {
            await Task.Delay(0);

            queue.Remove(key);
            entries.Remove(key);
        }

        public bool IsInitialized()
        {
            return ((entries == null) || (queue == null));
        }
    }
}
