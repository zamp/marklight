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
        private ElementAlignment _alignment;
        private ElementOrientation _orientation;
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
        /// Update RectTransform information.
        /// </summary>
        private void UpdateRectData() {

            if (!_isRectDirty)
                return;

            _isRectDirty = false;

            _layoutRect.CalculateInto(this);
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
                if (value)
                    _isRectDirty = true;
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
        /// Calculate the pixel Width of the view based on the LayoutData width value.
        /// </summary>
        public float PixelWidth
        {
            get
            {
                if (Width.Unit == ElementSizeUnit.Pixels)
                    return Width.Pixels;

                var parent = Parent;
                if (parent != null)
                    return parent.PixelWidth * Width.Percent;

                var camera = Camera.main;
                return camera != null
                    ? camera.pixelWidth * Width.Percent
                    : 0f;
            }
        }
        /// <summary>
        /// Calculate the pixel Height of the view based on the LayoutData height value.
        /// </summary>
        public float PixelHeight
        {
            get
            {
                if (Height.Unit == ElementSizeUnit.Pixels)
                    return Height.Pixels;

                var parent = Parent;
                if (parent != null)
                    return parent.PixelHeight * Height.Percent;

                var camera = Camera.main;
                return camera != null
                    ? camera.pixelHeight * Height.Percent
                    : 0f;
            }
        }

        /// <summary>
        /// Calculate the pixel Width of the inner part of the view based on the LayoutData width,
        /// margin and padding.
        /// </summary>
        public float InnerPixelWidth
        {
            get
            {
                var margins = -Margin.Left.Pixels - Margin.Right.Pixels
                               - Padding.Left.Pixels - Padding.Right.Pixels;

                if (Width.Unit == ElementSizeUnit.Pixels)
                    return Width.Pixels + margins;

                var parent = Parent;
                if (parent != null)
                    return (parent.InnerPixelWidth + margins) * Width.Percent;

                var camera = Camera.main;
                return camera != null
                    ? (camera.pixelWidth + margins) * Width.Percent
                    : 0f;
            }
        }

        /// <summary>
        /// Calculate the pixel Height of the inner part of the view based on the LayoutData height,
        /// margin and padding.
        /// </summary>
        public float InnerPixelHeight
        {
            get
            {
                var margins = -Margin.Top.Pixels - Margin.Bottom.Pixels
                              - Padding.Top.Pixels - Padding.Bottom.Pixels;

                if (Height.Unit == ElementSizeUnit.Pixels)
                    return Height.Pixels + margins;

                var parent = Parent;
                if (parent != null)
                    return (parent.InnerPixelHeight + margins) * Height.Percent;

                var camera = Camera.main;
                return camera != null
                    ? (camera.pixelHeight + margins) * Height.Percent
                    : 0f;
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
    }
}