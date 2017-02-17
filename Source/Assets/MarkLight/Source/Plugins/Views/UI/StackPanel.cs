using System.Collections.Generic;
using System.Linq;

namespace MarkLight.Views.UI
{
    /// <summary>
    /// Stack Panel view.
    /// </summary>
    /// <d>The StackPanel arranges child views next to each other either horizontally or vertically with no spacing.
    /// The child elements are sized to fill the StackPanel.</d>
    [HideInPresenter]
    public class StackPanel : UIView
    {
        #region Fields

        /// <summary>
        /// Orientation of the group.
        /// </summary>
        /// <d>The orientation of the group that determines how the child views will be arranged.</d>
        [ChangeHandler("LayoutChanged")]
        public _ElementOrientation Orientation;

        /// <summary>
        /// Sort direction.
        /// </summary>
        /// <d>If children has SortIndex set the sort direction determines which order the views should be arranged
        /// in the group.</d>
        [ChangeHandler("LayoutChanged")]
        public _ElementSortDirection SortDirection;

        /// <summary>
        /// Sets the visibility of children so they are made visible after they are arranged.
        /// </summary>
        /// <d>Boolean indicating that the group should set the visibility of children so they are only made visible
        /// after they are arranged.</d>
        public _bool SetChildVisibility;

        protected View _groupContentContainer;

        #endregion

        #region Methods

        public override void SetDefaultValues()
        {
            base.SetDefaultValues();

            Orientation.DirectValue = ElementOrientation.Vertical;
            SortDirection.DirectValue = ElementSortDirection.Ascending;
        }

        public override bool CalculateLayoutChanges(LayoutChangeContext context) {

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

                if (isHorizontal)
                {
                    HorizontalLayoutCalc.ApplyTo(view.Layout, childIndex, childCount);
                }
                else
                {
                    VerticalLayoutCalc.ApplyTo(view.Layout, childIndex, childCount);
                }

                // don't group disabled views
                if (!view.IsLive)
                {
                    if (SetChildVisibility)
                    {
                        view.IsVisible.Value = false;
                    }
                    continue;
                }

                // set offsets and alignment
                var offset = new ElementMargin(
                    new ElementSize(0f),
                    new ElementSize(0f));

                view.Layout.OffsetFromParent = offset;

                // set desired alignment if it is valid for the orientation otherwise use defaults
                view.Layout.Alignment = isHorizontal
                    ? ElementAlignment.Left
                    : ElementAlignment.Top;

                if (isHorizontal && !view.Height.IsSet)
                {
                    view.Layout.Height = new ElementSize(1f, ElementSizeUnit.Percents) { Fill = true };
                }
                else if (!isHorizontal && !view.Width.IsSet)
                {
                    view.Layout.Width = new ElementSize(1f, ElementSizeUnit.Percents) { Fill = true };
                }

                // update child layout
                context.NotifyLayoutUpdated(view);
                ++childIndex;

                // update child visibility
                if (SetChildVisibility)
                {
                    view.IsVisible.Value = true;
                }
            }

            if (!Width.IsSet)
                Layout.Width = new ElementSize(1f, ElementSizeUnit.Percents) { Fill = true };

            if (!Height.IsSet)
                Layout.Height = new ElementSize(1f, ElementSizeUnit.Percents) { Fill = true };

            if (!PropagateChildLayoutChanges.IsSet)
                PropagateChildLayoutChanges.DirectValue = false;

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

        #region Classes

        /// <summary>
        /// Horizontal orientation rect calculator
        /// </summary>
        private class HorizontalLayoutCalc : LayoutRectCalculator
        {
            private int _index;
            private int _count;

            /// <summary>
            /// Set horizontal calculator onto a views LayoutData, specifying the index position of the
            /// view and the views count.
            /// </summary>
            public static void ApplyTo(LayoutData data, int index, int count)
            {
                var calc = data.RectCalculator as HorizontalLayoutCalc;
                if (calc == null)
                {
                    calc = new HorizontalLayoutCalc();
                    data.RectCalculator = calc;
                }

                data.IsDirty = data.IsDirty || calc._index != index || calc._count != count;
                calc._index = index;
                calc._count = count;
            }

            protected override MinMax GetMinMaxX(LayoutData data, ElementSize width)
            {
                return new MinMax
                {
                    Min = _index / (float) _count,
                    Max = _index / (float) _count + 1 / (float) _count
                };
            }
        }

        /// <summary>
        /// Vertical orientation rect calculator
        /// </summary>
        private class VerticalLayoutCalc : LayoutRectCalculator
        {
            private int _index;
            private int _count;

            /// <summary>
            /// Set vertical calculator onto a views LayoutData, specifying the index position of the
            /// view and the views count.
            /// </summary>
            public static void ApplyTo(LayoutData data, int index, int count)
            {
                var calc = data.RectCalculator as VerticalLayoutCalc;
                if (calc == null)
                {
                    calc = new VerticalLayoutCalc();
                    data.RectCalculator = calc;
                }

                data.IsDirty = data.IsDirty || calc._index != index || calc._count != count;
                calc._index = index;
                calc._count = count;
            }

            protected override MinMax GetMinMaxY(LayoutData data, ElementSize height)
            {
                return new MinMax
                {
                    Min = 1f - (_index / (float) _count + 1 / (float) _count),
                    Max = 1f - _index / (float) _count
                };
            }
        }

        #endregion
    }
}