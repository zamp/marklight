using System.Collections.Generic;
using MarkLight.Views.UI;

namespace MarkLight
{
    /// <summary>
    /// Uniformly stacks child elements vertically or horizontally.
    /// </summary>
    public class UniformStackLayoutCalculator : LayoutCalculator
    {
        #region Fields

        /// <summary>
        /// Child layout orientation.
        /// </summary>
        public ElementOrientation Orientation;

        /// <summary>
        /// Get or set the visibility of children so they are made visible after they are arranged.
        /// </summary>
        public bool SetChildVisibility;

        #endregion

        #region Methods

        public override bool CalculateLayoutChanges(UIView view, IList<UIView> children, LayoutChangeContext context)
        {
            var isHorizontal = Orientation == ElementOrientation.Horizontal;

            // get size of content and set content offsets and alignment
            var childCount = children.Count;
            var childIndex = 0;

            for (var i = 0; i < childCount; ++i)
            {
                var child = children[i];

                if (child == null || child.IsDestroyed || !CanEffectChild(child))
                    continue;

                if (isHorizontal)
                {
                    HorizontalLayoutCalc.ApplyTo(child.Layout, childIndex, childCount);
                }
                else
                {
                    VerticalLayoutCalc.ApplyTo(child.Layout, childIndex, childCount);
                }

                // don't group disabled views
                if (!child.IsLive)
                {
                    if (SetChildVisibility)
                    {
                        child.IsVisible.Value = false;
                    }
                    continue;
                }

                // set offsets and alignment
                var offset = new ElementMargin(
                    new ElementSize(0f),
                    new ElementSize(0f));

                child.Layout.OffsetFromParent = offset;

                // set desired alignment if it is valid for the orientation otherwise use defaults
                child.Layout.Alignment = isHorizontal
                    ? ElementAlignment.Left
                    : ElementAlignment.Top;

                if (isHorizontal && !child.Height.IsSet)
                {
                    child.Layout.TargetHeight = new ElementSize(1f, ElementSizeUnit.Percents, true);
                }
                else if (!isHorizontal && !child.Width.IsSet)
                {
                    child.Layout.TargetWidth = new ElementSize(1f, ElementSizeUnit.Percents, true);
                }

                // update child layout
                context.NotifyLayoutUpdated(child);
                ++childIndex;

                // update child visibility
                if (SetChildVisibility)
                {
                    child.IsVisible.Value = true;
                }
            }

            if (!view.Width.IsSet)
                view.Layout.TargetWidth = ElementSize.FromPercents(1f, true);

            if (!view.Height.IsSet)
                view.Layout.TargetHeight = ElementSize.FromPercents(1f, true);

            return view.Layout.IsDirty;
        }

        #endregion

        #region Properties

        public override bool IsChildLayout
        {
            get { return true; }
        }

        public override bool IsAffectedByChildren
        {
            get { return false; }
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