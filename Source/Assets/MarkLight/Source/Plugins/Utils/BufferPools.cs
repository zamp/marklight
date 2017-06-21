using System.Text.RegularExpressions;
using MarkLight.Views.UI;

namespace MarkLight
{
    public static class BufferPools
    {
        public static readonly ListPool<View> ViewLists = new ListPool<View>();
        public static readonly ListPool<UIView> UIViewLists = new ListPool<UIView>();
        public static readonly ListPool<string> StringLists = new ListPool<string>();
        public static readonly HashSetPool<ViewFieldData> ViewFieldHashSets = new HashSetPool<ViewFieldData>();
        public static readonly ListPool<Match> MatchLists = new ListPool<Match>();
        public static readonly StringBuilderPool StringBuilders = new StringBuilderPool();
    }
}