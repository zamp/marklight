using System.Collections.Generic;
using MarkLight.Views.UI;

namespace MarkLight
{
    /// <summary>
    /// Provides access to a view pool.
    /// </summary>
    public class VirtualizedItems
    {
        #region Fields

        public VirtualizedItemsContainer VirtualizedItemsContainer;
        public ElementOrientation Orientation;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public VirtualizedItems(VirtualizedItemsContainer virtualizedItemsContainer)
        {
            VirtualizedItemsContainer = virtualizedItemsContainer;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inserts a view into the virtualized container.
        /// </summary>
        public void InsertView(ListItem view)
        {
            view.MoveTo(VirtualizedItemsContainer, -1, false);
        }

        /// <summary>
        /// Gets items that are in the given range.
        /// </summary>
        public List<ListItem> GetItemsInRange(float min, float max)
        {
            var items = new List<ListItem>();
            VirtualizedItemsContainer.ForEachChild<ListItem>(x =>
            {
                // see if item falls within the range
                if (IsInRange(x, min, max))
                {
                    items.Add(x);
                }
            }, ViewSearchArgs.NonRecursive);

            return items;
        }

        /// <summary>
        /// Gets boolean indicating if list item is in the specified range.
        /// </summary>
        public bool IsInRange(ListItem item, float min, float max)
        {
            if (Orientation == ElementOrientation.Vertical)
            {
                var itemMin = item.Layout.OffsetFromParent.Top.Pixels;
                var itemMax = itemMin + item.Layout.Height.Pixels;

                return itemMax >= min && itemMin <= max;
            }
            else
            {
                var itemMin = item.Layout.OffsetFromParent.Left.Pixels;
                var itemMax = itemMin + item.Layout.Width.Pixels;

                return itemMax >= min && itemMin <= max;
            }
        }

        #endregion

        #region Properties

        #endregion
    }
}
