using System.Collections.Generic;
using MarkLight.Views.UI;

namespace MarkLight
{
    /// <summary>
    /// Uniformly stacks child elements vertically or horizontally.
    /// </summary>
    public class UniformStackLayoutCalculator : LayoutCalculator
    {
        #region Methods

        public override bool CalculateLayoutChanges(UIView parent, IList<UIView> children, LayoutChangeContext context)
        {
            var isHorizontal = Orientation == ElementOrientation.Horizontal;

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

            if (!parent.Width.IsSet)
                parent.Layout.Width = new ElementSize(1f, ElementSizeUnit.Percents) { Fill = true };

            if (!parent.Height.IsSet)
                parent.Layout.Height = new ElementSize(1f, ElementSizeUnit.Percents) { Fill = true };

            return parent.Layout.IsDirty;
        }

        #endregion

        #region Properties

        public override bool IsChildLayout
        {
            get { return true; }
        }

        /// <summary>
        /// Child layout orientation.
        /// </summary>
        public ElementOrientation Orientation
        {
            get; set;
        }

        /// <summary>
        /// Get or set the visibility of children so they are made visible after they are arranged.
        /// </summary>
        public bool SetChildVisibility
        {
            get; set;
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