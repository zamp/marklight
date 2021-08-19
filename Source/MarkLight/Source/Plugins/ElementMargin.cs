using System;
using UnityEngine;

namespace MarkLight
{
    /// <summary>
    /// Represents left, top, right and bottom margins.
    /// </summary>
    [Serializable]
    public struct ElementMargin
    {
        #region Fields

        [SerializeField]
        private ElementSize _left;

        [SerializeField]
        private ElementSize _top;

        [SerializeField]
        private ElementSize _right;

        [SerializeField]
        private ElementSize _bottom;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ElementMargin(ElementMargin other)
        {
            _left = new ElementSize(other.Left);
            _top = new ElementSize(other.Top);
            _right = new ElementSize(other.Right);
            _bottom = new ElementSize(other.Bottom);
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ElementMargin(ElementSize margin)
        {
            _left = margin;
            _top = margin;
            _right = margin;
            _bottom = margin;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ElementMargin(ElementSize left, ElementSize top)
        {
            _left = left;
            _top = top;
            _right = new ElementSize();
            _bottom = new ElementSize();
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ElementMargin(ElementSize left, ElementSize top, ElementSize right)
            : this()
        {
            _left = left;
            _top = top;
            _right = right;
            _bottom = new ElementSize();
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ElementMargin(ElementSize left, ElementSize top, ElementSize right, ElementSize bottom)
        {
            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create copy with modified left size.
        /// </summary>
        public ElementMargin SetLeft(ElementSize left)
        {
            return new ElementMargin(left, Top, Right, Bottom);
        }

        /// <summary>
        /// Create copy with modified top size.
        /// </summary>
        public ElementMargin SetTop(ElementSize top)
        {
            return new ElementMargin(Left, top, Right, Bottom);
        }

        /// <summary>
        /// Create copy with modified right size.
        /// </summary>
        public ElementMargin SetRight(ElementSize right)
        {
            return new ElementMargin(Left, Top, right, Bottom);
        }

        /// <summary>
        /// Create copy with modified bottom size.
        /// </summary>
        public ElementMargin SetBottom(ElementSize bottom)
        {
            return new ElementMargin(Left, Top, Right, bottom);
        }

        /// <summary>
        /// Gets left margin from left size.
        /// </summary>
        public static ElementMargin FromLeft(ElementSize left)
        {
            return new ElementMargin(left, new ElementSize(), new ElementSize(), new ElementSize());
        }

        /// <summary>
        /// Gets top margin from top size.
        /// </summary>
        public static ElementMargin FromTop(ElementSize top)
        {
            return new ElementMargin(new ElementSize(), top, new ElementSize(), new ElementSize());
        }

        /// <summary>
        /// Gets left and top margin from left and top size.
        /// </summary>
        public static ElementMargin FromLeftTop(ElementSize left, ElementSize top)
        {
            return new ElementMargin(left, top, new ElementSize(), new ElementSize());
        }

        /// <summary>
        /// Gets right margin from right size.
        /// </summary>
        public static ElementMargin FromRight(ElementSize right)
        {
            return new ElementMargin(new ElementSize(), new ElementSize(), right, new ElementSize());
        }

        /// <summary>
        /// Gets bottom margin from bottom size.
        /// </summary>
        public static ElementMargin FromBottom(ElementSize bottom)
        {
            return new ElementMargin(new ElementSize(), new ElementSize(), new ElementSize(), bottom);
        }

        /// <summary>
        /// Gets right and bottom margin from right and bottom size.
        /// </summary>
        public static ElementMargin FromRightBottom(ElementSize right, ElementSize bottom)
        {
            return new ElementMargin(new ElementSize(), new ElementSize(), right, bottom);
        }

        /// <summary>
        /// Converts margin to string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", Left, Top, Right, Bottom);
        }

        public override int GetHashCode() {
            return (int)Left.Value ^ (int)Top.Value ^ (int)Right.Value ^ (int)Bottom.Value;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ElementMargin))
                return false;

            var other = (ElementMargin)obj;
            return other._left.Equals(_left) && other._top.Equals(_top)
                   && other._right.Equals(_right) && other._bottom.Equals(_bottom);
        }

        public static bool operator ==(ElementMargin margin1, ElementMargin margin2)
        {
            return margin1._left == margin2._left
                   && margin1._top == margin2._top
                   && margin1._right == margin2._right
                   && margin1._bottom == margin2._bottom;
        }

        public static bool operator !=(ElementMargin margin1, ElementMargin margin2)
        {
            return margin1._left != margin2._left
                   || margin1._top != margin2._top
                   || margin1._right != margin2._right
                   || margin1._bottom != margin2._bottom;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets left margin.
        /// </summary>
        public ElementSize Left
        {
            get
            {
                return _left;
            }
        }

        /// <summary>
        /// Gets or sets top margin.
        /// </summary>
        public ElementSize Top
        {
            get
            {
                return _top;
            }
        }

        /// <summary>
        /// Gets or sets right margin.
        /// </summary>
        public ElementSize Right
        {
            get
            {
                return _right;
            }
        }

        /// <summary>
        /// Gets or sets bottom margin.
        /// </summary>
        public ElementSize Bottom
        {
            get
            {
                return _bottom;
            }
        }

        #endregion
    }
}
