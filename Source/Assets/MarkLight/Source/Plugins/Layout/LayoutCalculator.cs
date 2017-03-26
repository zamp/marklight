using System.Collections.Generic;
using System.Collections.ObjectModel;
using MarkLight.Views.UI;

namespace MarkLight
{
    /// <summary>
    /// Abstract child layout calculator.
    /// </summary>
    public abstract class LayoutCalculator
    {
        private static readonly ReadOnlyCollection<UIView> EmptyList = new List<UIView>(0).AsReadOnly();

        /// <summary>
        /// Determine if the calculator also handles child layout.
        /// </summary>
        public abstract bool IsChildLayout { get; }

        /// <summary>
        /// Determine if the calculator is affected or can be affected by child layout;
        /// </summary>
        public abstract bool IsAffectedByChildren { get; }

        /// <summary>
        /// Calculate view layout.
        /// </summary>
        /// <param name="view">The parent view.</param>
        /// <param name="children">The child views of the parent view.</param>
        /// <param name="context">The layout change context.</param>
        /// <returns>True if layout changes were made.</returns>
        public abstract bool CalculateLayoutChanges(UIView view, IList<UIView> children,
            LayoutChangeContext context);

        /// <summary>
        /// Calculate view layout.
        /// </summary>
        /// <param name="view">The parent view.</param>
        /// <param name="context">The layout change context.</param>
        /// <returns>True if layout changes were made.</returns>
        public virtual bool CalculateLayoutChanges(UIView view, LayoutChangeContext context) {
            return CalculateLayoutChanges(view, EmptyList, context);
        }

        /// <summary>
        /// Determine if a child view can be affected by layout.
        /// </summary>
        protected virtual bool CanEffectChild(View view)
        {
            var uiView = view as UIView;
            return uiView != null && uiView.Layout.PositionType != ElementPositionType.Absolute;
        }
    }
}