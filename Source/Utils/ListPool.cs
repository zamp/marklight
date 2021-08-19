using System.Collections.Generic;
using UnityEngine;

namespace MarkLight
{
    /// <summary>
    /// Buffer list storage.
    /// </summary>
    public class ListPool<T> : Pool<List<T>>
    {
        /// <summary>
        /// Get a buffer from the pool and add values to it.
        /// </summary>
        public List<T> Get(ICollection<T> values)
        {
            var result = Get();
            result.AddRange(values);
            return result;
        }

        protected override List<T> CreateNew()
        {
            return new List<T>();
        }

        protected override void OnRecycle(List<T> item)
        {
            item.Clear();
        }
    }
}