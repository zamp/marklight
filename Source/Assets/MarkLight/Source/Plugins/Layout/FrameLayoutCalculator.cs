using System.Collections.Generic;
using MarkLight.Views.UI;
using UnityEngine;

namespace MarkLight
{
    /// <summary>
    /// Layout calculator for a Frame. Resizes to fit content.
    /// </summary>
    public class FrameLayoutCalculator : LayoutCalculator
    {
        #region Fields

        private static readonly ElementMargin EmptyMargin = new ElementMargin();

        /// <summary>
        /// Determine if the Width should be affected.
        /// </summary>
        public bool AdjustToWidth;

        /// <summary>
        /// Determine if the Height should be affected.
        /// </summary>
        public bool AdjustToHeight;

        /// <summary>
        /// The margin of the content.
        /// </summary>
        public ElementMargin ContentMargin;

        #endregion

        #region Methods

        public override bool CalculateLayoutChanges(UIView parent, IList<UIView> children, LayoutChangeContext context)
        {
            if (!AdjustToWidth && !AdjustToHeight)
                return parent.Layout.IsDirty;

            var maxWidth = 0f;
            var maxHeight = 0f;
            var childCount = children.Count;

            // get size of content and set content offsets and alignment
            for (var i = 0; i < childCount; ++i)
            {
                var child = children[i];
                if (child == null)
                    continue;

                // get size of content

                var width = child.Layout.AspectPixelWidth
                            + child.Layout.HorizontalOffsetPixels;

                var height = child.Layout.AspectPixelHeight
                            + child.Layout.VerticalOffsetPixels;

                if (child.Layout.PositionType != ElementPositionType.Absolute)
                {
                    width += child.Layout.HorizontalOffsetFromParentPixels;
                    height += child.Layout.VerticalOffsetFromParentPixels;
                }

                maxWidth = Mathf.Max(maxWidth, width) ;
                maxHeight = Mathf.Max(maxHeight, height);
            }

            var contentMargin = ContentMargin ?? EmptyMargin;

            // add margins
            maxWidth += contentMargin.Left.Pixels + contentMargin.Right.Pixels;
            maxHeight += contentMargin.Bottom.Pixels + contentMargin.Top.Pixels;

            // adjust size to content unless it has been set
            if (AdjustToWidth)
            {
                parent.Layout.Width = new ElementSize(maxWidth);
            }

            if (AdjustToHeight)
            {
                parent.Layout.Height = new ElementSize(maxHeight);
            }

            return parent.Layout.IsDirty;
        }

        #endregion

        #region Properties

        public override bool IsChildLayout
        {
            get { return true; }
        }

        public override bool IsAffectedByChildren
        {
            get { return true; }
        }

        #endregion
    }
}