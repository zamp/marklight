using System.Collections.Generic;

namespace MarkLight
{
    /// <summary>
    /// Buffer HashSet storage.
    /// </summary>
    public class HashSetPool<T> : Pool<HashSet<T>>
    {
        /// <summary>
        /// Get a buffer from the pool and add values to it,
        /// </summary>
        public HashSet<T> Get(ICollection<T> values)
        {
            var result = Get();
            result.AddRange(values);
            return result;
        }

        protected override HashSet<T> CreateNew()
        {
            return new HashSet<T>();
        }

        protected override void OnRecycle(HashSet<T> item)
        {
            item.Clear();
        }
    }
}