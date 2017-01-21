#region Using Statements
using MarkLight.ValueConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#endregion

namespace MarkLight.Views.UI
{
    /// <summary>
    /// Group view.
    /// </summary>
    /// <d>The group is used to spacially arrange child views next to each other either horizontally or vertically.</d>
    [HideInPresenter]
    public class Group : UIView
    {
        #region Fields

        /// <summary>
        /// Orientation of the group.
        /// </summary>
        /// <d>The orientation of the group that determines how the child views will be arranged.</d>
        [ChangeHandler("LayoutChanged")]
        public _ElementOrientation Orientation;

        /// <summary>
        /// Spacing between views.
        /// </summary>
        /// <d>Determines the spacing to be added between views in the group.</d>
        [ChangeHandler("LayoutChanged")]
        public _ElementSize Spacing;

        /// <summary>
        /// Content alignment.
        /// </summary>
        /// <d>Determines how the children should be aligned in relation to each other.</d>
        [ChangeHandler("LayoutChanged")]
        public _ElementAlignment ContentAlignment;

        /// <summary>
        /// Sort direction.
        /// </summary>
        /// <d>If children has SortIndex set the sort direction determines which order the views should be arranged in the group.</d>
        [ChangeHandler("LayoutChanged")]
        public _ElementSortDirection SortDirection;

        /// <summary>
        /// Sets the visibility of children so they are made visible after they are arranged.
        /// </summary>
        /// <d>Boolean indicating that the group should set the visibility of children so they are only made visible after they are arranged.</d>
        public _bool SetChildVisibility;                     

        protected View _groupContentContainer;

        #endregion

        #region Methods

        /// <summary>
        /// Sets default values of the view.
        /// </summary>
        public override void SetDefaultValues()
        {
            base.SetDefaultValues();

            Spacing.DirectValue = new ElementSize();
            Orientation.DirectValue = ElementOrientation.Vertical;
            SortDirection.DirectValue = ElementSortDirection.Ascending;
        }

        public override bool CalculateLayoutChanges(LayoutChangeContext context) {

            var maxWidth = 0f;
            var maxHeight = 0f;
            var totalWidth = 0f;
            var totalHeight = 0f;

            var isHorizontal = Orientation == ElementOrientation.Horizontal;

            var children = new List<UIView>();
            var childrenToBeSorted = new List<UIView>();
            _groupContentContainer.ForEachChild<UIView>(x =>
            {
                // should this be sorted?
                if (x.SortIndex != 0)
                {
                    // yes.
                    childrenToBeSorted.Add(x);
                    return;
                }

                children.Add(x);
            }, false);

            children.AddRange(SortDirection == ElementSortDirection.Ascending
                ? childrenToBeSorted.OrderBy(x => x.SortIndex.Value)
                : childrenToBeSorted.OrderByDescending(x => x.SortIndex.Value));

            // get size of content and set content offsets and alignment
            var childCount = children.Count;
            var childIndex = 0;
            for (var i = 0; i < childCount; ++i)
            {
                var view = children[i];

                // don't group disabled views
                if (!view.IsLive)
                {
                    if (SetChildVisibility)
                    {
                        view.IsVisible.Value = false;
                    }
                    continue;
                }

                var pixelWidth = view.Layout.Width.Unit == ElementSizeUnit.Percents
                    ? view.Layout.InnerPixelWidth
                    : view.Layout.Width.Pixels;

                var pixelHeight = view.Layout.Height.Unit == ElementSizeUnit.Percents
                    ? view.Layout.InnerPixelHeight
                    : view.Layout.Height.Pixels;

                // set offsets and alignment
                var offset = new ElementMargin(
                    new ElementSize(isHorizontal
                        ? totalWidth + Spacing.Value.Pixels * childIndex
                        : 0f, ElementSizeUnit.Pixels),
                    new ElementSize(!isHorizontal
                        ? totalHeight + Spacing.Value.Pixels * childIndex
                        : 0f, ElementSizeUnit.Pixels));

                view.OffsetFromParent.DirectValue = offset;

                // set desired alignment if it is valid for the orientation otherwise use defaults
                var alignment = isHorizontal
                    ? ElementAlignment.Left
                    : ElementAlignment.Top;

                var desiredAlignment = ContentAlignment.IsSet
                    ? ContentAlignment
                    : view.Alignment;

                if (isHorizontal && (desiredAlignment == ElementAlignment.Top
                                     || desiredAlignment == ElementAlignment.Bottom
                                     || desiredAlignment == ElementAlignment.TopLeft
                                     || desiredAlignment == ElementAlignment.BottomLeft))
                {
                    view.Layout.Alignment = alignment | desiredAlignment;
                }
                else if (!isHorizontal && (desiredAlignment == ElementAlignment.Left
                                           || desiredAlignment == ElementAlignment.Right
                                           || desiredAlignment == ElementAlignment.TopLeft
                                           || desiredAlignment == ElementAlignment.TopRight))
                {
                    view.Layout.Alignment = alignment | desiredAlignment;
                }
                else
                {
                    view.Layout.Alignment = alignment;
                }

                // get size of content
                totalWidth += pixelWidth;
                maxWidth = pixelWidth > maxWidth ? pixelWidth : maxWidth;

                totalHeight += pixelHeight;
                maxHeight = pixelHeight > maxHeight ? pixelHeight : maxHeight;

                // update child layout
                context.NotifyLayoutUpdated(view);
                ++childIndex;

                // update child visibility
                if (SetChildVisibility)
                {
                    view.IsVisible.Value = true;
                }
            }

            // set width and height
            var totalSpacing = childCount > 1
                ? (childIndex - 1) * Spacing.Value.Pixels
                : 0f;
            var adjustsToContent = false;

            if (!Width.IsSet)
            {
                // add margins
                totalWidth += isHorizontal ? totalSpacing : 0f;
                totalWidth += Layout.Margin.Left.Pixels + Layout.Margin.Right.Pixels;
                maxWidth += Layout.Margin.Left.Pixels + Layout.Margin.Right.Pixels;

                // adjust width to content
                Layout.Width = new ElementSize(isHorizontal ? totalWidth : maxWidth, ElementSizeUnit.Pixels);
                adjustsToContent = true;
            }

            if (!Height.IsSet)
            {
                // add margins
                totalHeight += !isHorizontal ? totalSpacing : 0f;
                totalHeight += Layout.Margin.Top.Pixels + Layout.Margin.Bottom.Pixels;
                maxHeight += Layout.Margin.Top.Pixels + Layout.Margin.Bottom.Pixels;

                // adjust height to content
                Layout.Height = new ElementSize(!isHorizontal ? totalHeight : maxHeight, ElementSizeUnit.Pixels);
                adjustsToContent = true;
            }

            if (!PropagateChildLayoutChanges.IsSet)
            {
                // don't propagate changes if width and height isn't adjusted to content
                PropagateChildLayoutChanges.DirectValue = adjustsToContent;
            }

            return Layout.IsDirty;
        }

        /// <summary>
        /// Initializes the view.
        /// </summary>
        public override void Initialize()
        {
            _groupContentContainer = this;
            if (SetChildVisibility)
            {
                // set inactive children as not visible
                _groupContentContainer.ForEachChild<UIView>(x =>
                {
                    if (!x.IsActive)
                    {
                        x.IsVisible.Value = false;
                    }
                }, false);
            }

            base.Initialize();            
        }
        
        #endregion
    }
}
