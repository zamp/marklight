using System.Collections.Generic;

namespace MarkLight
{
    /// <summary>
    /// Buffer stack storage.
    /// </summary>
    public class StackPool<T> : Pool<Stack<T>>
    {
        protected override Stack<T> CreateNew()
        {
            return new Stack<T>();
        }

        protected override void OnRecycle(Stack<T> item)
        {
            item.Clear();
        }
    }
}