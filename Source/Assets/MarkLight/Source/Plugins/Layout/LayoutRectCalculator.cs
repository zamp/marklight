using UnityEngine;

namespace MarkLight
{
    /// <summary>
    /// Calculates layout rect.
    /// </summary>
    public class LayoutRectCalculator
    {
        #region Fields

        /// <summary>
        /// Default instance.
        /// </summary>
        public static readonly LayoutRectCalculator Default = new LayoutRectCalculator();

        #endregion

        #region Methods

        /// <summary>
        /// Calculate rect using layout data and put results into layout data.
        /// </summary>
        public virtual void CalculateInto(LayoutData data) {

            // update rectTransform
            // horizontal alignment and positioning
            var width = data.View.OverrideWidth.IsSet ? data.View.OverrideWidth.Value : data.Width;
            var height = data.View.OverrideHeight.IsSet ? data.View.OverrideHeight.Value : data.Height;
            var margin = data.Margin;
            var offset = data.Offset;
            var offsetFromParent = data.OffsetFromParent;

            // horizontal alignment
            var minMaxX = GetMinMaxX(data, width);

            //  vertical alignment
            var minMaxY = GetMinMaxY(data, height);

            data.AnchorMin = new Vector2(minMaxX.Min + margin.Left.Percent, minMaxY.Min + margin.Bottom.Percent);
            data.AnchorMax = new Vector2(minMaxX.Max - margin.Right.Percent, minMaxY.Max - margin.Top.Percent);

            // positioning and margins
            data.OffsetMin = new Vector2(

                minMaxX.OffsetMin + margin.Left.Pixels + offset.Left.Pixels
                - offset.Right.Pixels + offsetFromParent.Left.Pixels
                - offsetFromParent.Right.Pixels,

                minMaxY.OffsetMin + margin.Bottom.Pixels - offset.Top.Pixels
                + offset.Bottom.Pixels - offsetFromParent.Top.Pixels
                + offsetFromParent.Bottom.Pixels);

            data.OffsetMax = new Vector2(

                minMaxX.OffsetMax - margin.Right.Pixels + offset.Left.Pixels
                - offset.Right.Pixels + offsetFromParent.Left.Pixels
                - offsetFromParent.Right.Pixels,

                minMaxY.OffsetMax - margin.Top.Pixels - offset.Top.Pixels
                + offset.Bottom.Pixels - offsetFromParent.Top.Pixels
                + offsetFromParent.Bottom.Pixels);

            data.AnchoredPosition = new Vector2(
                data.OffsetMin.x / 2.0f + data.OffsetMax.x / 2.0f,
                data.OffsetMin.y / 2.0f + data.OffsetMax.y / 2.0f);
        }

        /// <summary>
        /// Get horizontal rect min/max values.
        /// </summary>
        protected virtual MinMax GetMinMaxX(LayoutData data, ElementSize width) {

            var result = new MinMax();

            if (data.Alignment.HasFlag(ElementAlignment.Left))
            {
                result.Min = 0f;
                result.Max = width.Percent;
                result.OffsetMin = 0f;
                result.OffsetMax = width.Pixels;
            }
            else if (data.Alignment.HasFlag(ElementAlignment.Right))
            {
                result.Min = 1f - width.Percent;
                result.Max = 1f;
                result.OffsetMin = -width.Pixels;
                result.OffsetMax = 0f;
            }
            else
            {
                result.Min = 0.5f - width.Percent / 2f;
                result.Max = 0.5f + width.Percent / 2f;
                result.OffsetMin = -width.Pixels / 2f;
                result.OffsetMax = width.Pixels / 2f;
            }
            return result;
        }

        /// <summary>
        /// Get vertical rect min/max values.
        /// </summary>
        protected virtual MinMax GetMinMaxY(LayoutData data, ElementSize height) {

            //  vertical alignment
            var result = new MinMax();

            if (data.Alignment.HasFlag(ElementAlignment.Top))
            {
                result.Min = 1f - height.Percent;
                result.Max = 1f;
                result.OffsetMin = -height.Pixels;
                result.OffsetMax = 0f;
            }
            else if (data.Alignment.HasFlag(ElementAlignment.Bottom))
            {
                result.Min = 0f;
                result.Max = height.Percent;
                result.OffsetMin = 0f;
                result.OffsetMax = height.Pixels;
            }
            else
            {
                result.Min = 0.5f - height.Percent / 2f;
                result.Max = 0.5f + height.Percent / 2f;
                result.OffsetMin = -height.Pixels / 2f;
                result.OffsetMax = height.Pixels / 2f;
            }

            return result;
        }

        #endregion

        #region Struct

        protected struct MinMax
        {
            public float Min;
            public float Max;
            public float OffsetMin;
            public float OffsetMax;
        }

        #endregion
    }
}