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

            var pixelWidth = data.AspectPixelWidth;
            var pixelHeight = data.AspectPixelHeight;

            // positioning and margins
            data.OffsetMin = new Vector2(
                GetOffsetMinX(minMaxX.OffsetMin, pixelWidth, data),
                GetOffsetMinY(minMaxY.OffsetMin, pixelHeight, data));

            data.OffsetMax = new Vector2(
                GetOffsetMaxX(minMaxX.OffsetMax, pixelWidth, data),
                GetOffsetMaxY(minMaxY.OffsetMax, pixelHeight, data));

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
        protected virtual MinMax GetMinMaxY(LayoutData data, ElementSize height)
        {
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

        /// <summary>
        /// Get rect OffsetMin X component value. (Left)
        /// </summary>
        protected virtual float GetOffsetMinX(float offsetMin, float pixelWidth, LayoutData data)
        {
            var result = offsetMin
                   + data.MarginLeftPixels
                   + data.OffsetLeftPixels
                   - data.OffsetRightPixels
                   + data.OffsetFromParentLeftPixels
                   - data.OffsetFromParentRightPixels;

            //if (Math.Abs(data.MarginRightPixels) < 0.00001f)
            //    return result;

            var isLeftAligned = data.Alignment.HasFlag(ElementAlignment.Left);
            var isRightAligned = data.Alignment.HasFlag(ElementAlignment.Right);

            // make sure left and right edges do not overlap if there is room to move entire view

            var container = data.PaddedContainerPixelWidth - data.MarginLeftPixels;
            var width = pixelWidth + data.MarginRightPixels;

            // Positive delta means there is additional room to move the view in order to maintain width.
            var delta = container - width;
            if (delta > 0)
            {
                if (isLeftAligned)
                {
                    // Left aligned
                    result -= Mathf.Min(data.MarginRightPixels, delta);
                }
                else if (!isRightAligned)
                {
                    // Center aligned
                    result -= Mathf.Min(data.MarginRightPixels, delta) / 2f;
                }
            }
            else if (delta < 0 && data.Height.Unit == ElementSizeUnit.Pixels)
            {
                // force height of pixel unit value
                result -= Mathf.Min(data.MarginRightPixels, Mathf.Abs(delta));
            }

            return result;
        }

        /// <summary>
        /// Get rect OffsetMin Y component value. (Bottom)
        /// </summary>
        protected virtual float GetOffsetMinY(float offsetMin, float pixelHeight, LayoutData data)
        {
            var result = offsetMin
                   + data.MarginBottomPixels
                   - data.OffsetTopPixels
                   + data.OffsetBottomPixels
                   - data.OffsetFromParentTopPixels
                   + data.OffsetFromParentBottomPixels;

            var isTopAligned = data.Alignment.HasFlag(ElementAlignment.Top);
            var isBottomAligned = data.Alignment.HasFlag(ElementAlignment.Bottom);

            // make sure top and bottom edges do not overlap if there is room to move entire view
            var container = data.PaddedContainerPixelHeight - data.MarginBottomPixels;
            var height = pixelHeight + data.MarginTopPixels;

            // Positive delta means there is additional room to move the view in order to maintain height.
            var delta = container - height;
            if (delta > 0)
            {
                if (isTopAligned)
                {
                    // Top aligned
                    result -= Mathf.Min(data.MarginTopPixels, delta);
                }
                else if (!isBottomAligned)
                {
                    // Center aligned
                    result += Mathf.Min(data.MarginTopPixels, delta) / 2f;
                }
            }
            else if (delta < 0 && data.Height.Unit == ElementSizeUnit.Pixels)
            {
                // force height of pixel unit value
                result -= Mathf.Min(data.MarginTopPixels, Mathf.Abs(delta));
            }

            return result;
        }

        /// <summary>
        /// Get rect OffsetMax X component value. (Right)
        /// </summary>
        protected virtual float GetOffsetMaxX(float offsetMax, float pixelWidth, LayoutData data)
        {
            var result = offsetMax
                   - data.MarginRightPixels
                   + data.OffsetLeftPixels
                   - data.OffsetRightPixels
                   + data.OffsetFromParentLeftPixels
                   - data.OffsetFromParentRightPixels;

            var isLeftAligned = data.Alignment.HasFlag(ElementAlignment.Left);
            var isRightAligned = data.Alignment.HasFlag(ElementAlignment.Right);

            // make sure left and right edges do not overlap if there is room to move entire view

            var container = data.PaddedContainerPixelWidth - data.MarginRightPixels;
            var width = pixelWidth + data.MarginLeftPixels;

            // Positive delta means there is additional room to move the view in order to maintain width.
            var delta = container - width;
            if (delta > 0)
            {
                if (isLeftAligned)
                {
                    // Right aligned
                    result += Mathf.Min(data.MarginLeftPixels, delta);
                }
                else if (!isRightAligned)
                {
                    // Center aligned
                    result += Mathf.Min(data.MarginLeftPixels, delta) / 2f;
                }
            }
            else if (delta < 0 && data.Height.Unit == ElementSizeUnit.Pixels)
            {
                // force height of pixel unit value
                result += Mathf.Min(data.MarginLeftPixels, Mathf.Abs(delta));
            }

            return result;
        }

        /// <summary>
        /// Get rect OffsetMax Y component value. (Top)
        /// </summary>
        protected virtual float GetOffsetMaxY(float offsetMax, float pixelHeight, LayoutData data)
        {
            var result = offsetMax
                         - data.MarginTopPixels
                         - data.OffsetTopPixels
                         + data.OffsetBottomPixels
                         - data.OffsetFromParentTopPixels
                         + data.OffsetFromParentBottomPixels;

            var isTopAligned = data.Alignment.HasFlag(ElementAlignment.Top);
            var isBottomAligned = data.Alignment.HasFlag(ElementAlignment.Bottom);

            // make sure top and bottom edges do not overlap if there is room to move entire view
            var container = data.PaddedContainerPixelHeight - data.MarginTopPixels;
            var height = pixelHeight + data.MarginBottomPixels;

            // Positive delta means there is additional room to move the view in order to maintain height.
            var delta = container - height;
            if (delta > 0)
            {
                if (isBottomAligned)
                {
                    // Top aligned
                    result += Mathf.Min(data.MarginBottomPixels, delta);
                }
                else if (!isTopAligned)
                {
                    // Center aligned
                    result += Mathf.Min(data.MarginBottomPixels, delta) / 2f;
                }
            }
            else if (delta < 0 && data.Height.Unit == ElementSizeUnit.Pixels)
            {
                // force height of pixel unit value
                result += Mathf.Min(data.MarginBottomPixels, Mathf.Abs(delta));
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