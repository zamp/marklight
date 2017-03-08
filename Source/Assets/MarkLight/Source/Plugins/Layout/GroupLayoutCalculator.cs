using System.Collections.Generic;
using MarkLight.Views.UI;
using UnityEngine;

namespace MarkLight
{
    /// <summary>
    /// Arranges child view layout horizontally or vertically within a parent view.
    /// </summary>
    public class GroupLayoutCalculator : LayoutCalculator
    {

        #region Fields

        private float _horzSpacePixels;
        private float _vertSpacePixels;

        #endregion

        #region Methods

        public override bool CalculateLayoutChanges(UIView parent, IList<UIView> children, LayoutChangeContext context)
        {
            var parentLayout = parent.Layout;

            _horzSpacePixels = SizeToPixels(HorizontalSpacing, parentLayout);
            _vertSpacePixels = SizeToPixels(VerticalSpacing, parentLayout);

            var maxWidth = 0f;
            var maxHeight = 0f;
            var totalWidth = 0f;
            var totalHeight = 0f;

            var isHorizontal = Orientation == ElementOrientation.Horizontal;

            // get size of content and set content offsets and alignment
            var childCount = children.Count;
            var childIndex = 0;
            var firstItem = true;
            float xOffset = 0;
            float yOffset = 0;
            float maxColumnWidth = 0;
            float maxRowHeight = 0;

            var rows = new List<Row>();
            var row = new Row();

            var isLeftAligned = Alignment.HasFlag(ElementAlignment.Left);
            var isRightAligned = Alignment.HasFlag(ElementAlignment.Right);

            for (var i = 0; i < childCount; i++)
            {
                var viewIndex = isRightAligned ? childCount - 1 - i : i;
                var child = children[viewIndex];

                // don't group disabled or destroyed views
                if (!child.IsActive || child.IsDestroyed || !CanEffectChild(child))
                    continue;

                var pixelWidth = child.Layout.Width.Unit == ElementSizeUnit.Percents
                    ? child.Layout.InnerPixelWidth
                    : child.Layout.Width.Pixels;

                var pixelHeight = child.Layout.Height.Unit == ElementSizeUnit.Percents
                    ? child.Layout.InnerPixelHeight
                    : child.Layout.Height.Pixels;

                // set offsets and alignment
                var offset = new ElementMargin(
                    new ElementSize(
                        (isRightAligned ? -1 : 1) * (isHorizontal ? totalWidth + _horzSpacePixels * childIndex : 0f),
                        ElementSizeUnit.Pixels),
                    new ElementSize(!isHorizontal ? totalHeight + _vertSpacePixels * childIndex : 0f,
                        ElementSizeUnit.Pixels));

                child.Layout.OffsetFromParent = offset;
                child.Layout.Alignment = Alignment;

                if (Overflow == OverflowMode.Overflow)
                {
                    // get size of content
                    totalWidth += pixelWidth;
                    maxWidth = Mathf.Max(maxWidth, pixelWidth);

                    totalHeight += pixelHeight;
                    maxHeight = Mathf.Max(maxHeight, pixelHeight);

                    if (!isHorizontal)
                    {
                        if (row.Children.Count > 0)
                            rows.Add(row);

                        row = new Row();
                    }
                }
                else
                {
                    // overflow mode is set to wrap

                    // set offsets of item
                    if (isHorizontal)
                    {
                        if (firstItem)
                        {
                            // first item, don't check for overflow
                            xOffset = 0;
                            firstItem = false;
                        }
                        else if (xOffset + pixelWidth + _horzSpacePixels > parentLayout.InnerPixelWidth)
                        {
                            if (row.Children.Count > 0)
                                rows.Add(row);

                            row = new Row();

                            // item overflows to the next row
                            xOffset = 0;
                            yOffset += maxRowHeight + _vertSpacePixels;
                            maxRowHeight = 0;
                        }
                        else
                        {
                            // item continues on the same row
                            xOffset += _horzSpacePixels;
                        }

                        // set offset
                        child.Layout.OffsetFromParent = new ElementMargin(
                            ElementSize.FromPixels(xOffset),
                            ElementSize.FromPixels(yOffset));

                        xOffset += pixelWidth;
                        maxRowHeight = Mathf.Max(maxRowHeight, pixelHeight);
                        maxWidth = Mathf.Max(maxWidth, xOffset);
                        maxHeight = Mathf.Max(maxHeight, yOffset + pixelHeight);
                    }
                    else
                    {
                        if (row.Children.Count > 0)
                        {
                            rows.Add(row);
                            row = new Row();
                        }

                        if (firstItem)
                        {
                            yOffset = 0;
                            firstItem = false;
                        }
                        else if (yOffset + pixelHeight + _vertSpacePixels > parentLayout.InnerPixelHeight)
                        {
                            // overflow to next column
                            yOffset = 0;
                            xOffset += maxColumnWidth + _horzSpacePixels;
                            maxColumnWidth = 0;
                        }
                        else
                        {
                            // add spacing
                            yOffset += _vertSpacePixels;
                        }

                        // set offset
                        child.Layout.OffsetFromParent = new ElementMargin(
                            ElementSize.FromPixels(xOffset),
                            ElementSize.FromPixels(yOffset));

                        yOffset += pixelHeight;
                        maxColumnWidth = Mathf.Max(maxColumnWidth, pixelWidth);
                        maxWidth = Mathf.Max(maxWidth, xOffset + pixelWidth);
                        maxHeight = Mathf.Max(maxHeight, yOffset);
                    }
                }

                row.AddChild(child, pixelWidth, _horzSpacePixels);

                // update child layout
                context.NotifyLayoutUpdated(child);
                ++childIndex;
            }

            // horizontally centered
            if (!isLeftAligned && !isRightAligned)
            {
                if (row.Children.Count > 0)
                    rows.Add(row);

                // adjust left offset for center alignment
                foreach (var r in rows)
                {
                    var center = r.Width / 2f - r.FirstChildWidth / 2f;
                    foreach (var child in r.Children)
                    {
                        child.Layout.OffsetFromParent.Left.Value -= center;
                    }
                }
            }

            var updateScrollContent = false;

            if (Overflow == OverflowMode.Overflow)
            {
                // add margins
                totalWidth += isHorizontal
                    ? (childCount > 1 ? (childIndex - 1) * _horzSpacePixels : 0f)
                    : 0f;

                var widthMargins = GetHorizontalMargins(parentLayout);

                totalWidth += widthMargins;
                maxWidth += widthMargins;

                // set width and height of list
                if (!parent.Width.IsSet)
                {
                    // adjust width to content
                    parent.Layout.Width = new ElementSize(isHorizontal
                        ? totalWidth
                        : maxWidth, ElementSizeUnit.Pixels);
                }
                else if (ScrollContent != null)
                {
                    // adjust width of scrollable area to size
                    ScrollContent.Layout.Width = new ElementSize(isHorizontal
                        ? totalWidth
                        : maxWidth, ElementSizeUnit.Pixels);

                    updateScrollContent = true;
                }

                // add margins
                totalHeight += !isHorizontal
                    ? (childCount > 1 ? (childIndex - 1) * _vertSpacePixels : 0f)
                    : 0f;

                var heightMargins = GetVerticalMargins(parentLayout);

                totalHeight += heightMargins;
                maxHeight += heightMargins;

                if (!parent.Height.IsSet)
                {
                    // if height is not explicitly set then adjust to content
                    parent.Layout.Height = new ElementSize(isHorizontal
                        ? maxHeight
                        : totalHeight, ElementSizeUnit.Pixels);
                }
                else if (ScrollContent != null)
                {
                    // adjust width of scrollable area to size
                    ScrollContent.Layout.Height = new ElementSize(isHorizontal
                        ? maxHeight
                        : totalHeight, ElementSizeUnit.Pixels);

                    updateScrollContent = true;
                }
            }
            else // Wrap
            {
                // adjust size to content
                if (isHorizontal)
                {
                    maxHeight += GetVerticalMargins(parentLayout);

                    if (ScrollContent != null)
                    {
                        ScrollContent.Layout.Height = ElementSize.FromPixels(maxHeight);
                        updateScrollContent = true;
                    }
                    else if (!parent.Height.IsSet)
                    {
                        parentLayout.Height = ElementSize.FromPixels(maxHeight);
                    }
                }
                else // Vertical
                {
                    maxWidth += GetHorizontalMargins(parentLayout);

                    if (ScrollContent != null)
                    {
                        ScrollContent.Layout.Width = ElementSize.FromPixels(maxWidth);
                        updateScrollContent = true;
                    }
                    else if (!parent.Width.IsSet)
                    {
                        parentLayout.Width = ElementSize.FromPixels(maxWidth);
                    }
                }
            }

            if (updateScrollContent)
            {
                context.NotifyLayoutUpdated(ScrollContent);
            }

            return parentLayout.IsDirty;
        }

        #endregion

        #region Properties

        public override bool IsChildLayout
        {
            get { return true; }
        }

        /// <summary>
        /// Horizontal space between child elements.
        /// </summary>
        public ElementSize HorizontalSpacing
        {
            get; set;
        }

        /// <summary>
        /// Vertical space between child elements.
        /// </summary>
        public ElementSize VerticalSpacing
        {
            get; set;
        }

        /// <summary>
        /// Alignment of child elements.
        /// </summary>
        public ElementAlignment Alignment
        {
            get; set;
        }

        /// <summary>
        /// Child layout orientation.
        /// </summary>
        public ElementOrientation Orientation
        {
            get; set;
        }

        /// <summary>
        /// Get or set how child layout overflow is handled.
        /// </summary>
        public OverflowMode Overflow
        {
            get; set;
        }

        /// <summary>
        /// Get or set the scroll content view, if any.
        /// </summary>
        public UIView ScrollContent
        {
            get; set;
        }

        #endregion

        #region Classes

        /// <summary>
        /// Layout row data context.
        /// </summary>
        private class Row
        {
            public float Width;
            public float FirstChildWidth;
            public readonly List<UIView> Children = new List<UIView>(10);

            public void AddChild(UIView child, float width, float space) {

                if (Children.Count == 0)
                    FirstChildWidth = width;
                else
                    Width += space;

                Children.Add(child);
                Width += width;
            }
        }

        #endregion
    }
}