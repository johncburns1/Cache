using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace HACache
{
    public class CacheEntry
    {
        private object   value;
        private DateTime lastAccessed;
        private long     cacheKey;

        public CacheEntry(object value, long cacheKey = -1L)
        {
            this.value        = value;
            this.cacheKey     = cacheKey;
            this.lastAccessed = DateTime.UtcNow;
        }

        public object Value { get; set; }
        public DateTime LastAccessed { get; set; }
        public long CacheKey { get; set; }
    }
}
