using MarkLight.Views.UI;
using UnityEngine;

namespace MarkLight
{
    /// <summary>
    /// Stores layout working data. Views use and modify the layout information here so they
    /// don't need to overwrite user fields, losing data in the process.
    /// </summary>
    public class LayoutData
    {
        #region Fields

        /// <summary>
        /// The View the LayoutData belongs to.
        /// </summary>
        public readonly UIView View;

        private bool _isDirty;
        private ElementPositionType _positionType;
        private ElementAlignment _alignment;
        private ElementOrientation _orientation;
        private ElementAspectRatio _aspectRatio;
        private ElementSize _width = new ElementSize();
        private ElementSize _height = new ElementSize();
        private ElementMargin _offsetFromParent = new ElementMargin();
        private ElementMargin _offset = new ElementMargin();
        private ElementMargin _margin = new ElementMargin();
        private ElementMargin _padding = new ElementMargin();

        private LayoutRectCalculator _layoutRect = LayoutRectCalculator.Default;
        private Vector2 _anchorMin;
        private Vector2 _anchorMax;
        private Vector2 _offsetMin;
        private Vector2 _offsetMax;
        private Vector3 _anchoredPosition;
        private bool _isRectDirty = true;

        private PixelSizes _horizontalSizes;
        private PixelSizes _verticalSizes;
        private bool _isSizeDirty = true;

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public LayoutData(UIView view)
        {
            View = view;
        }

        #region Methods

        /// <summary>
        /// Convert an ElementSize to pixel value.
        /// </summary>
        /// <param name="size">The size to convert.</param>
        public float WidthToPixels(ElementSize size)
        {
            return SizeToPixels(size, PixelWidth);
        }

        /// <summary>
        /// Convert an ElementSize to pixel value.
        /// </summary>
        /// <param name="size">The size to convert.</param>
        public float HeightToPixels(ElementSize size)
        {
            return SizeToPixels(size, PixelHeight);
        }

        /// <summary>
        /// Convert and ElementSize to pixel value.
        /// </summary>
        /// <param name="size">The size to convert.</param>
        /// <param name="containerSize">The size of the element container.</param>
        public float SizeToPixels(ElementSize size, float containerSize)
        {
            return size != null
                ? size.Unit == ElementSizeUnit.Percents
                    ? containerSize * size.Percent
                    : size.Pixels
                : 0f;
        }

        /// <summary>
        /// Update the views RectTransform with layout data.
        /// </summary>
        public void UpdateRect(bool forceRecalculation = false)
        {
            if (forceRecalculation)
                IsDirty = true;

            IsDirty = false;

            RectCalculator.CalculateInto(this);

            View.RectTransform.anchorMin = AnchorMin;
            View.RectTransform.anchorMax = AnchorMax;

            // positioning and margins
            View.RectTransform.offsetMin = OffsetMin;
            View.RectTransform.offsetMax = OffsetMax;
            View.RectTransform.anchoredPosition = AnchoredPosition;
        }

        /// <summary>
        /// Update RectTransform information.
        /// </summary>
        private void UpdateRectData()
        {
            if (!_isRectDirty)
                return;

            _isRectDirty = false;

            _layoutRect.CalculateInto(this);
        }

        /// <summary>
        /// Update width, height, margin and padding data.
        /// </summary>
        private void UpdateSizeData()
        {
            if (!_isSizeDirty)
                return;

            var width = View.OverrideWidth.IsSet
                ? View.OverrideWidth.Value
                : Width;

            var height = View.OverrideHeight.IsSet
                ? View.OverrideHeight.Value
                : Height;

            _horizontalSizes = GetHorizontalSizes(width);
            _verticalSizes = GetVerticalSizes(height);

            _isSizeDirty = false;
        }

        /// <summary>
        /// Get Horizontal pixel sizes.
        /// </summary>
        private PixelSizes GetHorizontalSizes(ElementSize width)
        {
            var result = new PixelSizes();
            var containerWidth = ContainerPixelWidth;
            var isAbsolute = PositionType == ElementPositionType.Absolute;

            result.ContainerPadX = isAbsolute || Parent == null
                ? 0f
                : SizeToPixels(Parent.Padding.Left, containerWidth);

            result.ContainerPadY = isAbsolute || Parent == null
                ? 0f
                : SizeToPixels(Padding.Right, containerWidth);

            result.ContainerSize = containerWidth;

            if (width.Unit == ElementSizeUnit.Pixels)
            {
                result.IsExplicit = true;
                result.TargetSize = width.Pixels;
            }
            else
            {
                result.TargetSize = result.ContainerSize * width.Percent;
            }

            result.MarginX = SizeToPixels(Margin.Left, result.TargetSize);
            result.MarginY = SizeToPixels(Margin.Right, result.TargetSize);

            result.OffsetX = SizeToPixels(Offset.Left, containerWidth);
            result.OffsetY = SizeToPixels(Offset.Right, containerWidth);

            result.OffsetFromParentX = isAbsolute ? 0f : SizeToPixels(OffsetFromParent.Left, containerWidth);
            result.OffsetFromParentY = isAbsolute ? 0f : SizeToPixels(OffsetFromParent.Right, containerWidth);

            result.IsPositionExplicit = Margin.Left.Unit == ElementSizeUnit.Pixels
                                        && Margin.Right.Unit == ElementSizeUnit.Pixels
                                        && Offset.Left.Unit == ElementSizeUnit.Pixels
                                        && Offset.Right.Unit == ElementSizeUnit.Pixels
                                        && OffsetFromParent.Left.Unit == ElementSizeUnit.Pixels
                                        && OffsetFromParent.Right.Unit == ElementSizeUnit.Pixels;
            return result;
        }

        /// <summary>
        /// Get Vertical pixel sizes.
        /// </summary>
        private PixelSizes GetVerticalSizes(ElementSize height)
        {
            var result = new PixelSizes();
            var containerHeight = ContainerPixelHeight;
            var isAbsolute = PositionType == ElementPositionType.Absolute;

            result.ContainerPadX = isAbsolute || Parent == null
                ? 0f
                : SizeToPixels(Parent.Padding.Top, containerHeight);

            result.ContainerPadY = isAbsolute || Parent == null
                ? 0f
                : SizeToPixels(Parent.Padding.Bottom, containerHeight);

            result.ContainerSize = containerHeight;

            if (height.Unit == ElementSizeUnit.Pixels)
            {
                result.IsExplicit = true;
                result.TargetSize = height.Pixels;
            }
            else
            {
                result.TargetSize = result.ContainerSize * height.Percent;
            }

            result.MarginX = SizeToPixels(Margin.Top, result.TargetSize);
            result.MarginY = SizeToPixels(Margin.Bottom, result.TargetSize);

            result.OffsetX = SizeToPixels(Offset.Top, containerHeight);
            result.OffsetY = SizeToPixels(Offset.Bottom, containerHeight);

            result.OffsetFromParentX = isAbsolute ? 0f : SizeToPixels(OffsetFromParent.Top, containerHeight);
            result.OffsetFromParentY = isAbsolute ? 0f : SizeToPixels(OffsetFromParent.Bottom, containerHeight);

            result.IsPositionExplicit = Margin.Top.Unit == ElementSizeUnit.Pixels
                                        && Margin.Bottom.Unit == ElementSizeUnit.Pixels
                                        && Offset.Top.Unit == ElementSizeUnit.Pixels
                                        && Offset.Bottom.Unit == ElementSizeUnit.Pixels
                                        && OffsetFromParent.Top.Unit == ElementSizeUnit.Pixels
                                        && OffsetFromParent.Bottom.Unit == ElementSizeUnit.Pixels;

            return result;
        }

        /// <summary>
        /// Copy values from one ElementSize to another.
        /// </summary>
        public static void Copy(ElementSize from, ElementSize to)
        {
            to.Unit = from.Unit;
            to.Fill = from.Fill;
            to.Value = from.Value;
        }

        /// <summary>
        /// Copy values from one ElementMargin to another.
        /// </summary>
        public static void Copy(ElementMargin from, ElementMargin to)
        {
            Copy(from.Left, to.Left);
            Copy(from.Top, to.Top);
            Copy(from.Right, to.Right);
            Copy(from.Bottom, to.Bottom);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates a data value has been changed.
        /// </summary>
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                _isDirty = value;
                if (!value)
                    return;

                _isRectDirty = true;
                _isSizeDirty = true;

                // ensure parent sizes will be recalculated for accuracy
                var parent = Parent;
                while (parent != null && !parent.IsDirty)
                {
                    parent._isSizeDirty = true;
                    parent = parent.Parent;
                }
            }
        }

        /// <summary>
        /// Determine if width and height are both pixel unit values.
        /// </summary>
        public bool IsSizeExplicit
        {
            get
            {
                UpdateSizeData();
                return _horizontalSizes.IsExplicit && _verticalSizes.IsExplicit;
            }
        }

        /// <summary>
        /// Determine if positioning values such as Margin, Offset, and OffsetFromParent consist of only pixel units.
        /// </summary>
        public bool IsPositionExplicit
        {
            get
            {
                UpdateSizeData();
                return _horizontalSizes.IsPositionExplicit && _verticalSizes.IsPositionExplicit;
            }
        }

        /// <summary>
        /// Get the LayoutData from the views LayoutParent. Returns null if there is no parent.
        /// </summary>
        public LayoutData Parent
        {
            get
            {
                var view = View.LayoutParent as UIView;
                return view == null ? null : view.Layout;
            }
        }

        /// <summary>
        /// Get the container width in pixels.
        /// </summary>
        public float ContainerPixelWidth
        {
            get
            {
                float result;

                if (Parent != null)
                {
                    result = Parent.PixelWidth;
                }
                else
                {
                    var camera = Camera.main;
                    result = camera != null
                        ? camera.pixelWidth
                        : 0f;
                }

                return result;
            }
        }

        /// <summary>
        /// Get the container width in pixels and adjusted for padding.
        /// </summary>
        public float PaddedContainerPixelWidth
        {
            get
            {
                UpdateSizeData();
                return _horizontalSizes.PaddedContainerSize;
            }
        }

        /// <summary>
        /// Get the container height in pixels.
        /// </summary>
        public float ContainerPixelHeight
        {
            get
            {
                float result;

                if (Parent != null)
                {
                    result = Parent.PixelHeight;
                }
                else
                {
                    var camera = Camera.main;
                    result = camera != null
                        ? camera.pixelHeight
                        : 0f;
                }

                return result;
            }
        }

        /// <summary>
        /// Get the container height in pixels and adjusted for padding.
        /// </summary>
        public float PaddedContainerPixelHeight
        {
            get
            {
                UpdateSizeData();
                return _verticalSizes.PaddedContainerSize;
            }
        }

        /// <summary>
        /// Calculate the pixel Width of the view based on the LayoutData width, margin and padding. Does not adjust
        /// for AspectRatio field.
        /// </summary>
        public float PixelWidth
        {
            get
            {
                UpdateSizeData();
                return _horizontalSizes.Size;
            }
        }

        /// <summary>
        /// Calculate the pixel Height of the view based on the LayoutData height, margin and padding. Does not
        /// adjust for AspectRatio field.
        /// </summary>
        public float PixelHeight
        {
            get
            {
                UpdateSizeData();
                return _verticalSizes.Size;
            }
        }

        /// <summary>
        /// Get the width adjusted for AspectRatio value.
        /// </summary>
        public ElementSize AspectWidth
        {
            get
            {
                UpdateSizeData();

                if (Width.Unit == ElementSizeUnit.Pixels || !View.AspectRatio.IsSet)
                    return Width;

                var pixelHeight = _verticalSizes.Size;
                var aspectWidth = pixelHeight * (AspectRatio.X / AspectRatio.Y);

                if (Height.Unit == ElementSizeUnit.Pixels)
                {
                    return new ElementSize(aspectWidth + _horizontalSizes.Margins);
                }

                var pixelWidth = _horizontalSizes.Size;

                if (AspectRatio.X >= AspectRatio.Y)
                {
                    var aspectHeight = pixelWidth * (AspectRatio.Y / AspectRatio.X);
                    return aspectHeight > pixelHeight
                        ? new ElementSize(aspectWidth + _horizontalSizes.Margins)
                        : Width;
                }

                return aspectWidth <= pixelWidth
                    ? new ElementSize(aspectWidth + _horizontalSizes.Margins)
                    : Width;
            }
        }

        /// <summary>
        /// Get the width adjusted for AspectRatio value in pixels.
        /// </summary>
        public float AspectPixelWidth
        {
            get
            {
                var width = AspectWidth;
                return ReferenceEquals(width, Width)
                    ? PixelWidth
                    : width.Pixels;
            }
        }

        /// <summary>
        /// Get the height adjusted for AspectRatio value.
        /// </summary>
        public ElementSize AspectHeight
        {
            get
            {
                UpdateSizeData();

                if (Height.Unit == ElementSizeUnit.Pixels || !View.AspectRatio.IsSet)
                    return Height;

                var pixelWidth = _horizontalSizes.Size;
                var aspectHeight = pixelWidth * (AspectRatio.Y / AspectRatio.X);

                if (Width.Unit == ElementSizeUnit.Pixels)
                {
                    return new ElementSize(aspectHeight + _verticalSizes.Margins);
                }

                var pixelHeight = _verticalSizes.Size;

                if (AspectRatio.X >= AspectRatio.Y)
                {
                    return aspectHeight <= pixelHeight
                        ? new ElementSize(aspectHeight + _verticalSizes.Margins)
                        : Height;
                }

                var aspectWidth = pixelHeight * (AspectRatio.X / AspectRatio.Y);
                return aspectWidth > pixelWidth
                    ? new ElementSize(aspectHeight + _verticalSizes.Margins)
                    : Height;
            }
        }

        /// <summary>
        /// Get the height adjusted for AspectRatio value in pixels.
        /// </summary>
        public float AspectPixelHeight
        {
            get {
                var height = AspectHeight;
                return ReferenceEquals(height, Height)
                    ? PixelHeight
                    : height.Pixels;
            }
        }

        /// <summary>
        /// Get or set the layout positioning.
        /// </summary>
        public ElementPositionType PositionType
        {
            get { return _positionType; }
            set
            {
                if (_positionType == value)
                    return;

                _positionType = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Get or set the layout alignment. Setting value causes IsDirty field to be true.
        /// </summary>
        public ElementAlignment Alignment
        {
            get { return _alignment; }
            set
            {
                if (_alignment == value)
                    return;

                _alignment = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Get or set the layout orientation. Setting value causes IsDirty field to be true.
        /// </summary>
        public ElementOrientation Orientation
        {
            get { return _orientation; }
            set
            {
                if (_orientation == value)
                    return;

                _orientation = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Get or set the layout width. Setting value causes IsDirty field to be true.
        /// </summary>
        public ElementSize Width
        {
            get { return _width; }
            set
            {
                if (Equals(_width, value))
                    return;

                _width = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Get or set the layout height. Setting value causes IsDirty field to be true.
        /// </summary>
        public ElementSize Height
        {
            get { return _height; }
            set
            {
                if (Equals(_height, value))
                    return;

                _height = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Get or set the aspect ratio used to adjust percent based width and height values.
        /// </summary>
        public ElementAspectRatio AspectRatio
        {
            get { return _aspectRatio; }
            set
            {
                if (Equals(_aspectRatio, value))
                    return;

                _aspectRatio = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Get or set the layout offset from parent. Setting value causes IsDirty field to be true.
        /// </summary>
        public ElementMargin OffsetFromParent
        {
            get { return _offsetFromParent; }
            set
            {
                if (Equals(_offsetFromParent, value))
                    return;

                _offsetFromParent = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Get the left and right offset in pixels.
        /// </summary>
        public float HorizontalOffsetFromParentPixels
        {
            get
            {
                UpdateSizeData();
                return _horizontalSizes.OffsetsFromParent;
            }
        }

        /// <summary>
        /// Get the top and bottom offset in pixels
        /// </summary>
        public float VerticalOffsetFromParentPixels
        {
            get
            {
                UpdateSizeData();
                return _verticalSizes.OffsetsFromParent;
            }
        }

        /// <summary>
        /// Get the layout left offset in pixels.
        /// </summary>
        public float OffsetFromParentLeftPixels
        {
            get
            {
                UpdateSizeData();
                return _horizontalSizes.OffsetFromParentX;
            }
        }

        /// <summary>
        /// Get the layout top offset in pixels.
        /// </summary>
        public float OffsetFromParentTopPixels
        {
            get
            {
                UpdateSizeData();
                return _verticalSizes.OffsetFromParentX;
            }
        }

        /// <summary>
        /// Get the layout right offset in pixels.
        /// </summary>
        public float OffsetFromParentRightPixels
        {
            get
            {
                UpdateSizeData();
                return _horizontalSizes.OffsetFromParentY;
            }
        }

        /// <summary>
        /// Get the layout bottom offset in pixels.
        /// </summary>
        public float OffsetFromParentBottomPixels
        {
            get
            {
                UpdateSizeData();
                return _verticalSizes.OffsetFromParentY;
            }
        }


        /// <summary>
        /// Get or set the layout offset. Setting value causes IsDirty field to be true.
        /// </summary>
        public ElementMargin Offset
        {
            get { return _offset; }
            set
            {
                if (Equals(_offset, value))
                    return;

                _offset = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Get the left and right offset in pixels.
        /// </summary>
        public float HorizontalOffsetPixels
        {
            get
            {
                UpdateSizeData();
                return _horizontalSizes.Offsets;
            }
        }

        /// <summary>
        /// Get the top and bottom offset in pixels
        /// </summary>
        public float VerticalOffsetPixels
        {
            get
            {
                UpdateSizeData();
                return _verticalSizes.Offsets;
            }
        }

        /// <summary>
        /// Get the layout left offset in pixels.
        /// </summary>
        public float OffsetLeftPixels
        {
            get
            {
                UpdateSizeData();
                return _horizontalSizes.OffsetX;
            }
        }

        /// <summary>
        /// Get the layout top offset in pixels.
        /// </summary>
        public float OffsetTopPixels
        {
            get
            {
                UpdateSizeData();
                return _verticalSizes.OffsetX;
            }
        }

        /// <summary>
        /// Get the layout right offset in pixels.
        /// </summary>
        public float OffsetRightPixels
        {
            get
            {
                UpdateSizeData();
                return _horizontalSizes.OffsetY;
            }
        }

        /// <summary>
        /// Get the layout bottom offset in pixels.
        /// </summary>
        public float OffsetBottomPixels
        {
            get
            {
                UpdateSizeData();
                return _verticalSizes.OffsetY;
            }
        }

        /// <summary>
        /// Get or set the layout margin. Setting value causes IsDirty field to be true.
        /// </summary>
        public ElementMargin Margin
        {
            get { return _margin; }
            set
            {
                if (Equals(_margin, value))
                    return;

                _margin = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Get the left and right margin in pixels.
        /// </summary>
        public float HorizontalMarginPixels
        {
            get
            {
                UpdateSizeData();
                return _horizontalSizes.Margins;
            }
        }

        /// <summary>
        /// Get the top and bottom margin in pixels
        /// </summary>
        public float VerticalMarginPixels
        {
            get
            {
                UpdateSizeData();
                return _verticalSizes.Margins;
            }
        }

        /// <summary>
        /// Get the layout left margin in pixels.
        /// </summary>
        public float MarginLeftPixels
        {
            get
            {
                UpdateSizeData();
                return _horizontalSizes.MarginX;
            }
        }

        /// <summary>
        /// Get the layout top margin in pixels.
        /// </summary>
        public float MarginTopPixels
        {
            get
            {
                UpdateSizeData();
                return _verticalSizes.MarginX;
            }
        }

        /// <summary>
        /// Get the layout right margin in pixels.
        /// </summary>
        public float MarginRightPixels
        {
            get
            {
                UpdateSizeData();
                return _horizontalSizes.MarginY;
            }
        }

        /// <summary>
        /// Get the layout bottom margin in pixels.
        /// </summary>
        public float MarginBottomPixels
        {
            get
            {
                UpdateSizeData();
                return _verticalSizes.MarginY;
            }
        }

        /// <summary>
        /// Get or set the layout padding. Setting value causes IsDirty field to be true.
        /// </summary>
        public ElementMargin Padding
        {
            get { return _padding; }
            set
            {
                if (Equals(_padding, value))
                    return;

                _padding = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Get the left and right padding in pixels.
        /// </summary>
        public float HorizontalPaddingPixels
        {
            get { return PaddingLeftPixels + PaddingRightPixels; }
        }

        /// <summary>
        /// Get the top and bottom margin in pixels
        /// </summary>
        public float VerticalPaddingPixels
        {
            get { return PaddingTopPixels + PaddingBottomPixels; }
        }

        /// <summary>
        /// Get the layout left padding in pixels.
        /// </summary>
        public float PaddingLeftPixels
        {
            get { return WidthToPixels(_padding.Left); }
        }

        /// <summary>
        /// Get the layout top padding in pixels.
        /// </summary>
        public float PaddingTopPixels
        {
            get { return HeightToPixels(_padding.Top); }
        }

        /// <summary>
        /// Get the layout right padding in pixels.
        /// </summary>
        public float PaddingRightPixels
        {
            get { return WidthToPixels(_padding.Right); }
        }

        /// <summary>
        /// Get the layout bottom padding in pixels.
        /// </summary>
        public float PaddingBottomPixels
        {
            get { return HeightToPixels(_padding.Bottom); }
        }

        /// <summary>
        /// Get or set the layout rect calculator.
        /// </summary>
        public LayoutRectCalculator RectCalculator
        {
            get { return _layoutRect; }
            set
            {
                _layoutRect = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// The normalized position in the parent RectTransform that the lower left corner is anchored to.
        /// </summary>
        public Vector2 AnchorMin
        {
            get
            {
                UpdateRectData();
                return _anchorMin;
            }
            set { _anchorMin = value; }
        }

        /// <summary>
        /// The normalized position in the parent RectTransform that the upper right corner is anchored to.
        /// </summary>
        public Vector2 AnchorMax
        {
            get
            {
                UpdateRectData();
                return _anchorMax;
            }
            set { _anchorMax = value; }
        }

        /// <summary>
        /// The offset of the lower left corner of the rectangle relative to the lower left anchor.
        /// </summary>
        public Vector2 OffsetMin
        {
            get
            {
                UpdateRectData();
                return _offsetMin;
            }
            set { _offsetMin = value; }
        }

        /// <summary>
        /// The offset of the upper right corner of the rectangle relative to the upper right anchor.
        /// </summary>
        public Vector2 OffsetMax
        {
            get
            {
                UpdateRectData();
                return _offsetMax;
            }
            set { _offsetMax = value; }
        }

        /// <summary>
        /// The position of the pivot of this RectTransform relative to the anchor reference point.
        /// </summary>
        public Vector3 AnchoredPosition
        {
            get
            {
                UpdateRectData();
                return _anchoredPosition;
            }
            set { _anchoredPosition = value; }
        }

        #endregion

        #region Structs

        private struct PixelSizes
        {
            public bool IsExplicit;
            public bool IsPositionExplicit;

            public float TargetSize;

            public float ContainerSize;
            public float ContainerPadX;
            public float ContainerPadY;

            public float MarginX;
            public float MarginY;

            public float OffsetX;
            public float OffsetY;

            public float OffsetFromParentX;
            public float OffsetFromParentY;

            public float ContainerPadding
            {
                get { return ContainerPadX + ContainerPadY; }
            }

            public float Margins
            {
                get { return MarginX + MarginY; }
            }

            public float Offsets
            {
                get { return OffsetX + OffsetY; }
            }

            public float OffsetsFromParent
            {
                get { return OffsetFromParentX + OffsetFromParentY; }
            }

            public float PaddedContainerSize
            {
                get { return ContainerSize - ContainerPadding; }
            }

            public float Size
            {
                get
                {
                    if (IsExplicit)
                        return TargetSize;

                    return Mathf.Min(TargetSize,
                        ContainerSize - Margins,
                        PaddedContainerSize);
                }
            }
        }

        #endregion
    }
}