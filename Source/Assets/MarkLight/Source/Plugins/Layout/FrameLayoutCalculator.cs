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

        #endregion

        #region Methods

        public override bool CalculateLayoutChanges(UIView parent, IList<UIView> children, LayoutChangeContext context)
        {
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

                var width = Mathf.Max(child.Layout.PixelWidth, child.Layout.Width.Pixels)
                            + WidthToPixels(child.Layout.Offset.Left, parent.Layout)
                            + WidthToPixels(child.Layout.Offset.Right, parent.Layout);

                var height = Mathf.Max(child.Layout.PixelHeight, child.Layout.Height.Pixels)
                            + HeightToPixels(child.Layout.Offset.Top, parent.Layout)
                            + HeightToPixels(child.Layout.Offset.Bottom, parent.Layout);

                if (child.Layout.PositionType != ElementPositionType.Absolute)
                {
                    width += WidthToPixels(child.Layout.OffsetFromParent.Left, parent.Layout);
                    width += WidthToPixels(child.Layout.OffsetFromParent.Right, parent.Layout);

                    height += HeightToPixels(child.Layout.OffsetFromParent.Top, parent.Layout);
                    height += HeightToPixels(child.Layout.OffsetFromParent.Bottom, parent.Layout);
                }

                maxWidth = Mathf.Max(maxWidth, width) ;
                maxHeight = Mathf.Max(maxHeight, height);
            }

            var contentMargin = ContentMargin ?? EmptyMargin;

            // add margins
            maxWidth += contentMargin.Left.Pixels + contentMargin.Right.Pixels;
            maxHeight += contentMargin.Bottom.Pixels + contentMargin.Top.Pixels;

            // adjust size to content unless it has been set
            if (!parent.Width.IsSet)
            {
                parent.Layout.Width = new ElementSize(maxWidth);
            }

            if (!parent.Height.IsSet)
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

        public ElementMargin ContentMargin
        {
            get; set;
        }

        #endregion
    }
}