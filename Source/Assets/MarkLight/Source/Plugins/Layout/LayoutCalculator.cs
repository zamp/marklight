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
        /// Calculate view layout.
        /// </summary>
        /// <param name="parent">The parent view.</param>
        /// <param name="children">The child views of the parent view.</param>
        /// <param name="context">The layout change context.</param>
        /// <returns>True if layout changes were made.</returns>
        public abstract bool CalculateLayoutChanges(UIView parent, IList<UIView> children,
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
        /// Get the sum of all horizontal margins and padding.
        /// </summary>
        /// <param name="layout">The layout data to get margins and padding from.</param>
        protected virtual float GetHorizontalMargins(LayoutData layout)
        {
            return layout.HorizontalMarginPixels + layout.HorizontalPaddingPixels;
        }

        /// <summary>
        /// Get the sum of all vertical margins and padding.
        /// </summary>
        /// <param name="layout">The layout data to get margins and padding from.</param>
        protected virtual float GetVerticalMargins(LayoutData layout)
        {
            return layout.VerticalMarginPixels + layout.VerticalPaddingPixels;
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