using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Marklight.Themes
{
    public class StyleCollection : KeyedCollection<Style, Style>
    {
        public readonly string Selector;

        public StyleCollection(string selector)
        {
            Selector = selector;
        }

        protected override Style GetKeyForItem(Style item) {
            return item;
        }
    }
}