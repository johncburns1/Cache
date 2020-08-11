using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HACache
{
    public class MemoryCache : CacheBase
    {
        public MemoryCache(EvictionPolicy evictionPolicy)
            : base(evictionPolicy)
        {
        }

        public override async Task<object> Add(object key, object value)
        {
            await semaphore.WaitAsync();

            try
            {
                if (!IsInitialized())
                {
                    return null;
                }

                if (!entries.ContainsKey(key))
                {
                    if (queue.Count == capacity)
                    {
                        // evict if we are at capacity

                        var evicted = evictionPolicy.Evict(queue);
                        entries.Remove(evicted);
                    }
                }
                else
                {
                    // handle duplicates

                    queue.Remove(key);
                }

                // add new entry locally

                queue.AddFirst(key);
                entries[key] = new CacheEntry(value);

                return key;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
