namespace MarkLight
{
    /// <summary>
    /// Reusable item storage.
    /// </summary>
    public class ItemPool<T> : Pool<T> where T : new()
    {
        protected override T CreateNew()
        {
            return new T();
        }
    }
}