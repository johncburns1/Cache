using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HACache
{
    public interface EvictionPolicy
    {
        public object Evict(LinkedList<object> queue);
    }

    public class LRUEviction : EvictionPolicy
    {
        public object Evict(LinkedList<object> queue)
        {
            var last = queue.Last();
            queue.RemoveLast();

            return last;
        }
    }

    public class RandomEviction : EvictionPolicy
    {
        public object Evict(LinkedList<object> queue)
        {
            var random = new Random();
            int index  = random.Next(queue.Count);
            var entry  = queue.ElementAt(index);

            queue.Remove(entry);

            return entry;
        }
    }
}
