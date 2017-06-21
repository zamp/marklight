using System.Collections.Generic;

namespace MarkLight
{
    /// <summary>
    /// Simple abstract generic object pool.
    /// </summary>
    public abstract class Pool<T>
    {
        private List<T> _pool;

        /// <summary>
        /// Get a buffer from the pool.
        /// </summary>
        public T Get()
        {
            if (_pool == null || _pool.Count == 0)
                return CreateNew();

            if (_pool == null)
                _pool = new List<T>();

            var index = _pool.Count - 1;
            var result = _pool[index];
            _pool.RemoveAt(index);

            return result;
        }

        /// <summary>
        /// Return a buffer back to the pool.
        /// </summary>
        public void Recycle(T item)
        {
            if (_pool == null)
                _pool = new List<T>();

            _pool.Add(item);
            OnRecycle(item);
        }

        protected abstract T CreateNew();

        protected virtual void OnRecycle(T item)
        { }
    }
}