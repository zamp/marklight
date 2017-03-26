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
        public virtual void CalculateInto(LayoutData data)
        {
            // update rectTransform
            // horizontal alignment and positioning
            var width = data.View.OverrideWidth.IsSet
                ? data.View.OverrideWidth.Value
                : data.AspectWidth;

            var height = data.View.OverrideHeight.IsSet
                ? data.View.OverrideHeight.Value
                : data.AspectHeight;

            // horizontal alignment
            var minMaxX = GetMinMaxX(data, width);

            //  vertical alignment
            var minMaxY = GetMinMaxY(data, height);

            data.AnchorMin = new Vector2(minMaxX.Min, minMaxY.Min);
            data.AnchorMax = new Vector2(minMaxX.Max, minMaxY.Max);

            // positioning and margins
            data.OffsetMin = new Vector2(
                GetOffsetMinX(minMaxX.OffsetMin, data),
                GetOffsetMinY(minMaxY.OffsetMin, data));

            data.OffsetMax = new Vector2(
                GetOffsetMaxX(minMaxX.OffsetMax, data),
                GetOffsetMaxY(minMaxY.OffsetMax, data));

            data.AnchoredPosition = new Vector2(
                data.OffsetMin.x / 2.0f + data.OffsetMax.x / 2.0f,
                data.OffsetMin.y / 2.0f + data.OffsetMax.y / 2.0f);
        }

        /// <summary>
        /// Get horizontal rect min/max values.
        /// </summary>
        protected virtual MinMax GetMinMaxX(LayoutData data, ElementSize width)
        {
            var result = new MinMax();
            var pixelWidth = width.Unit == ElementSizeUnit.Pixels
                ? width.Pixels + data.HorizontalMarginPixels
                : 0f;

            if (data.Alignment.HasFlag(ElementAlignment.Left))
            {
                result.Min = 0f;
                result.Max = width.Percent;
                result.OffsetMin = 0f;
                result.OffsetMax = pixelWidth;
            }
            else if (data.Alignment.HasFlag(ElementAlignment.Right))
            {
                result.Min = 1f - width.Percent;
                result.Max = 1f;
                result.OffsetMin = -pixelWidth;
                result.OffsetMax = 0f;
            }
            else
            {
                result.Min = 0.5f - width.Percent / 2f;
                result.Max = 0.5f + width.Percent / 2f;
                result.OffsetMin = -pixelWidth / 2f;
                result.OffsetMax = pixelWidth / 2f;
            }
            return result;
        }

        /// <summary>
        /// Get vertical rect min/max values.
        /// </summary>
        protected virtual MinMax GetMinMaxY(LayoutData data, ElementSize height)
        {
            //  vertical alignment
            var result = new MinMax();
            var pixelHeight = height.Unit == ElementSizeUnit.Pixels
                ? height.Pixels + data.VerticalMarginPixels
                : 0f;

            if (data.Alignment.HasFlag(ElementAlignment.Top))
            {
                result.Min = 1f - height.Percent;
                result.Max = 1f;
                result.OffsetMin = -pixelHeight;
                result.OffsetMax = 0f;
            }
            else if (data.Alignment.HasFlag(ElementAlignment.Bottom))
            {
                result.Min = 0f;
                result.Max = height.Percent;
                result.OffsetMin = 0f;
                result.OffsetMax = pixelHeight;
            }
            else
            {
                result.Min = 0.5f - height.Percent / 2f;
                result.Max = 0.5f + height.Percent / 2f;
                result.OffsetMin = -pixelHeight / 2f;
                result.OffsetMax = pixelHeight / 2f;
            }

            return result;
        }

        /// <summary>
        /// Get rect OffsetMin X component value. (Left)
        /// </summary>
        protected virtual float GetOffsetMinX(float offsetMin, LayoutData data)
        {
            return offsetMin
                   + data.MarginLeftPixels
                   + data.OffsetLeftPixels
                   - data.OffsetRightPixels
                   + data.OffsetFromParentLeftPixels
                   - data.OffsetFromParentRightPixels;
        }

        /// <summary>
        /// Get rect OffsetMin Y component value. (Bottom)
        /// </summary>
        protected virtual float GetOffsetMinY(float offsetMin, LayoutData data)
        {
            return offsetMin
                   + data.MarginBottomPixels
                   - data.OffsetTopPixels
                   + data.OffsetBottomPixels
                   - data.OffsetFromParentTopPixels
                   + data.OffsetFromParentBottomPixels;
        }

        /// <summary>
        /// Get rect OffsetMax X component value. (Right)
        /// </summary>
        protected virtual float GetOffsetMaxX(float offsetMax, LayoutData data)
        {
            return offsetMax
                   - data.MarginRightPixels
                   + data.OffsetLeftPixels
                   - data.OffsetRightPixels
                   + data.OffsetFromParentLeftPixels
                   - data.OffsetFromParentRightPixels;
        }

        /// <summary>
        /// Get rect OffsetMax Y component value. (Top)
        /// </summary>
        protected virtual float GetOffsetMaxY(float offsetMax, LayoutData data)
        {
            return offsetMax
                   - data.MarginTopPixels
                   - data.OffsetTopPixels
                   + data.OffsetBottomPixels
                   - data.OffsetFromParentTopPixels
                   + data.OffsetFromParentBottomPixels;
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