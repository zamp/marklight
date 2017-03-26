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

            result.MarginXPixels = SizeToPixels(Margin.Left, result.TargetSize);
            result.MarginYPixels = SizeToPixels(Margin.Right, result.TargetSize);

            result.OffsetXPixels = SizeToPixels(Offset.Left, containerWidth);
            result.OffsetYPixels = SizeToPixels(Offset.Right, containerWidth);

            result.OffsetFromParentXPixels = isAbsolute ? 0f : SizeToPixels(OffsetFromParent.Left, containerWidth);
            result.OffsetFromParentYPixels = isAbsolute ? 0f : SizeToPixels(OffsetFromParent.Right, containerWidth);

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

            result.MarginXPixels = SizeToPixels(Margin.Top, result.TargetSize);
            result.MarginYPixels = SizeToPixels(Margin.Bottom, result.TargetSize);

            result.OffsetXPixels = SizeToPixels(Offset.Top, containerHeight);
            result.OffsetYPixels = SizeToPixels(Offset.Bottom, containerHeight);

            result.OffsetFromParentXPixels = isAbsolute ? 0f : SizeToPixels(OffsetFromParent.Top, containerHeight);
            result.OffsetFromParentYPixels = isAbsolute ? 0f : SizeToPixels(OffsetFromParent.Bottom, containerHeight);

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
        /// Calculate the pixel Width of the view based on the LayoutData width, margin and padding. Does not adjust
        /// for AspectRatio field.
        /// </summary>
        public float PixelWidth
        {
            get
            {
                UpdateSizeData();
                return _horizontalSizes.BoxSize;
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
                return _verticalSizes.BoxSize;
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

                var pixelHeight = _verticalSizes.BoxSize;
                var aspectWidth = pixelHeight * (AspectRatio.X / AspectRatio.Y);

                if (Height.Unit == ElementSizeUnit.Pixels)
                {
                    return new ElementSize(aspectWidth);
                }

                var pixelWidth = _horizontalSizes.BoxSize;

                if (AspectRatio.X >= AspectRatio.Y)
                {
                    var aspectHeight = pixelWidth * (AspectRatio.Y / AspectRatio.X);
                    return aspectHeight > pixelHeight
                        ? new ElementSize(aspectWidth)
                        : Width;
                }

                return aspectWidth <= pixelWidth
                    ? new ElementSize(aspectWidth)
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

                var pixelWidth = _horizontalSizes.BoxSize;
                var aspectHeight = pixelWidth * (AspectRatio.Y / AspectRatio.X);

                if (Width.Unit == ElementSizeUnit.Pixels)
                {
                    return new ElementSize(aspectHeight);
                }

                var pixelHeight = _verticalSizes.BoxSize;

                if (AspectRatio.X >= AspectRatio.Y)
                {
                    return aspectHeight <= pixelHeight
                        ? new ElementSize(aspectHeight)
                        : Height;
                }

                var aspectWidth = pixelHeight * (AspectRatio.X / AspectRatio.Y);
                return aspectWidth > pixelWidth
                    ? new ElementSize(aspectHeight)
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
        /// Get the pixel width of area displaced by view within its container.
        /// </summary>
        public float PixelDisplacementWidth
        {
            get
            {
                UpdateSizeData();
                return _horizontalSizes.BoxSize + _horizontalSizes.MarginPixels;
            }
        }

        /// <summary>
        /// Get the pixel height of area displaced by view within its container.
        /// </summary>
        public float PixelDisplacementHeight
        {
            get
            {
                UpdateSizeData();
                return _verticalSizes.BoxSize + _verticalSizes.MarginPixels;
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
                return _horizontalSizes.OffsetFromParentPixels;
            }
        }

        /// <summary>
        /// Get the left and right offset as a percent.
        /// </summary>
        public float HorizontalOffsetFromParentPercent
        {
            get
            {
                UpdateSizeData();
                return _horizontalSizes.OffsetFromParentPercent;
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
                return _verticalSizes.OffsetFromParentPixels;
            }
        }

        /// <summary>
        /// Get the top and bottom offset as a percent.
        /// </summary>
        public float VerticalOffsetFromParentPercent
        {
            get
            {
                UpdateSizeData();
                return _verticalSizes.OffsetFromParentPercent;
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
                return _horizontalSizes.OffsetFromParentXPixels;
            }
        }

        /// <summary>
        /// Get the layout left offset as a percent.
        /// </summary>
        public float OffsetFromParentLeftPercent
        {
            get
            {
                UpdateSizeData();
                return _horizontalSizes.OffsetFromParentXPercent;
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
                return _verticalSizes.OffsetFromParentXPixels;
            }
        }

        /// <summary>
        /// Get the layout top offset as a percent.
        /// </summary>
        public float OffsetFromParentTopPercent
        {
            get
            {
                UpdateSizeData();
                return _verticalSizes.OffsetFromParentXPercent;
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
                return _horizontalSizes.OffsetFromParentYPixels;
            }
        }

        /// <summary>
        /// Get the layout right offset as a percent.
        /// </summary>
        public float OffsetFromParentRightPercent
        {
            get
            {
                UpdateSizeData();
                return _horizontalSizes.OffsetFromParentYPercent;
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
                return _verticalSizes.OffsetFromParentYPixels;
            }
        }

        /// <summary>
        /// Get the layout bottom offset as a percent.
        /// </summary>
        public float OffsetFromParentBottomPercent
        {
            get
            {
                UpdateSizeData();
                return _verticalSizes.OffsetFromParentYPercent;
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
                return _horizontalSizes.OffsetPixels;
            }
        }

        /// <summary>
        /// Get the left and right offset as a percent.
        /// </summary>
        public float HorizontalOffsetPercent
        {
            get
            {
                UpdateSizeData();
                return _horizontalSizes.OffsetPercent;
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
                return _verticalSizes.OffsetPixels;
            }
        }

        /// <summary>
        /// Get the top and bottom offset as a percent.
        /// </summary>
        public float VerticalOffsetPercent
        {
            get
            {
                UpdateSizeData();
                return _verticalSizes.OffsetPercent;
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
                return _horizontalSizes.OffsetXPixels;
            }
        }

        /// <summary>
        /// Get the layout left offset as a percent.
        /// </summary>
        public float OffsetLeftPercent
        {
            get
            {
                UpdateSizeData();
                return _horizontalSizes.OffsetXPercent;
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
                return _verticalSizes.OffsetXPixels;
            }
        }

        /// <summary>
        /// Get the layout top offset as a percent.
        /// </summary>
        public float OffsetTopPercent
        {
            get
            {
                UpdateSizeData();
                return _verticalSizes.OffsetXPercent;
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
                return _horizontalSizes.OffsetYPixels;
            }
        }

        /// <summary>
        /// Get the layout right offset as a percent.
        /// </summary>
        public float OffsetRightPercent
        {
            get
            {
                UpdateSizeData();
                return _horizontalSizes.OffsetYPercent;
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
                return _verticalSizes.OffsetYPixels;
            }
        }

        /// <summary>
        /// Get the layout bottom offset as a percent.
        /// </summary>
        public float OffsetBottomPercent
        {
            get
            {
                UpdateSizeData();
                return _verticalSizes.OffsetYPercent;
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
                return _horizontalSizes.MarginPixels;
            }
        }

        /// <summary>
        /// Get the left and right margin as a percent.
        /// </summary>
        public float HorizontalMarginPercent
        {
            get
            {
                UpdateSizeData();
                return _horizontalSizes.MarginPercent;
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
                return _verticalSizes.MarginPixels;
            }
        }

        /// <summary>
        /// Get the top and bottom margin as a percent.
        /// </summary>
        public float VerticalMarginPercent
        {
            get
            {
                UpdateSizeData();
                return _verticalSizes.MarginPercent;
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
                return _horizontalSizes.MarginXPixels;
            }
        }

        /// <summary>
        /// Get the layout left margin as a percent.
        /// </summary>
        public float MarginLeftPercent
        {
            get
            {
                UpdateSizeData();
                return _horizontalSizes.MarginXPercent;
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
                return _verticalSizes.MarginXPixels;
            }
        }

        /// <summary>
        /// Get the layout top margin as a percent.
        /// </summary>
        public float MarginTopPercent
        {
            get
            {
                UpdateSizeData();
                return _verticalSizes.MarginXPercent;
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
                return _horizontalSizes.MarginYPixels;
            }
        }

        /// <summary>
        /// Get the layout right margin as a percent.
        /// </summary>
        public float MarginRightPercent
        {
            get
            {
                UpdateSizeData();
                return _horizontalSizes.MarginYPercent;
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
                return _verticalSizes.MarginYPixels;
            }
        }

        /// <summary>
        /// Get the layout bottom margin as a percent.
        /// </summary>
        public float MarginBottomPercent
        {
            get
            {
                UpdateSizeData();
                return _verticalSizes.MarginYPercent;
            }
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

            public float MarginXPixels;
            public float MarginYPixels;

            public float OffsetXPixels;
            public float OffsetYPixels;

            public float OffsetFromParentXPixels;
            public float OffsetFromParentYPixels;

            public float DisplacementLeftTop
            {
                get
                {
                    return TargetSize
                           + MarginPixels
                           + OffsetXPixels
                           + OffsetFromParentPixels
                           - OffsetYPixels
                           - OffsetFromParentPixels;
                }
            }

            public float DisplacementRightBottom
            {
                get
                {
                    return Mathf.Max(0f, TargetSize
                           + MarginPixels
                           - OffsetXPixels
                           - OffsetFromParentPixels
                           + OffsetYPixels
                           + OffsetFromParentPixels);
                }
            }

            public float DisplacementCenter
            {
                get
                {
                    // centered
                    return Mathf.Max(0f, TargetSize
                           + MarginPixels
                           - OffsetXPixels
                           - OffsetFromParentPixels
                           - OffsetYPixels
                           - OffsetFromParentPixels);
                }
            }

            public float MarginPixels
            {
                get { return MarginXPixels + MarginYPixels; }
            }

            public float MarginPercent
            {
                get { return MarginXPercent + MarginYPercent; }
            }

            public float MarginXPercent
            {
                get { return MarginXPixels / TargetSize; }
            }

            public float MarginYPercent
            {
                get { return MarginYPixels / TargetSize; }
            }

            public float OffsetPixels
            {
                get { return OffsetXPixels + OffsetYPixels; }
            }

            public float OffsetPercent
            {
                get { return OffsetXPercent + OffsetYPercent; }
            }

            public float OffsetXPercent
            {
                get { return OffsetXPixels / ContainerSize; }
            }

            public float OffsetYPercent
            {
                get { return OffsetYPixels / ContainerSize; }
            }

            public float OffsetFromParentPixels
            {
                get { return OffsetFromParentXPixels + OffsetFromParentYPixels; }
            }

            public float OffsetFromParentPercent
            {
                get { return OffsetFromParentXPercent + OffsetFromParentYPercent; }
            }

            public float OffsetFromParentXPercent
            {
                get { return OffsetFromParentXPixels / ContainerSize; }
            }

            public float OffsetFromParentYPercent
            {
                get { return OffsetFromParentYPixels / ContainerSize; }
            }

            public float BoxSize
            {
                get
                {
                    if (IsExplicit)
                        return TargetSize;

                    return Mathf.Min(TargetSize,
                        ContainerSize - MarginPixels);
                }
            }
        }

        #endregion
    }
}