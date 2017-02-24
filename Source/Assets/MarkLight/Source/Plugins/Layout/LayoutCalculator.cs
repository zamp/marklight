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
            return SizeToPixels(layout.Margin.Left, layout)
                   + SizeToPixels(layout.Margin.Right, layout)
                   + SizeToPixels(layout.Padding.Left, layout)
                   + SizeToPixels(layout.Padding.Right, layout);
        }

        /// <summary>
        /// Get the sum of all vertical margins and padding.
        /// </summary>
        /// <param name="layout">The layout data to get margins and padding from.</param>
        protected virtual float GetVerticalMargins(LayoutData layout)
        {
            return SizeToPixels(layout.Margin.Top, layout)
                   + SizeToPixels(layout.Margin.Bottom, layout)
                   + SizeToPixels(layout.Padding.Top, layout)
                   + SizeToPixels(layout.Padding.Bottom, layout);
        }

        /// <summary>
        /// Convert an ElementSize to pixel value.
        /// </summary>
        /// <param name="size">The size to convert.</param>
        /// <param name="data">The data to use for conversion.</param>
        protected float SizeToPixels(ElementSize size, LayoutData data)
        {
            return size != null
                ? size.Unit == ElementSizeUnit.Percents
                    ? data.PixelWidth * size.Percent
                    : size.Pixels
                : 0f;
        }
    }
}