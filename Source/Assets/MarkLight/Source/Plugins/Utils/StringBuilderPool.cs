using System.Text;

namespace MarkLight
{
    /// <summary>
    /// StringBuilder pooled storage.
    /// </summary>
    public class StringBuilderPool : Pool<StringBuilder>
    {
        protected override StringBuilder CreateNew()
        {
            return new StringBuilder(32);
        }

        protected override void OnRecycle(StringBuilder item)
        {
            item.Length = 0;
        }
    }
}