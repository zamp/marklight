using System;
using System.Globalization;
using System.Text;
using Marklight;
using UnityEngine;

namespace MarkLight
{
    /// <summary>
    /// Represents size in pixels, elements or percentage.
    /// </summary>
    [Serializable]
    public class ElementSize
    {
        #region Fields

        [SerializeField]
        private float _value;

        [SerializeField]
        private ElementSizeUnit _unit;

        [SerializeField]
        private bool _fill;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ElementSize()
        {
            _value = 0f;
            _unit = ElementSizeUnit.Pixels;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ElementSize(float pixels)
        {
            _value = pixels;
            _unit = ElementSizeUnit.Pixels;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ElementSize(float value, ElementSizeUnit unit)
        {
            _value = value;
            _unit = unit;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ElementSize(ElementSize elementSize)
        {
            _value = elementSize.Value;
            _unit = elementSize.Unit;
            _fill = elementSize.Fill;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts float to element size.
        /// </summary>
        public static implicit operator ElementSize(float value)
        {
            return FromPixels(value);
        }

        /// <summary>
        /// Gets element size with the specified pixel size.
        /// </summary>
        public static ElementSize FromPixels(float pixels)
        {
            return new ElementSize(pixels, ElementSizeUnit.Pixels);
        }

        /// <summary>
        /// Gets element size with the specified percent size (0.0 - 1.0).
        /// </summary>
        public static ElementSize FromPercents(float percents)
        {
            return new ElementSize(percents, ElementSizeUnit.Percents);
        }

        /// <summary>
        /// Parses string into element size.
        /// </summary>
        public static ElementSize Parse(string value, Vector3 unitSize, StringBuilder buffer = null)
        {
            value = value.Trim();

            var values = new ParsedNumber[1];
            ParseUtils.ParseDelimitedNumbers(value, values, -1, -1, buffer);
            return values[0].ToElementSize(unitSize);
        }

        /// <summary>
        /// Converts element size to string.
        /// </summary>
        public override string ToString() {
            if (Unit == ElementSizeUnit.Percents)
            {
                return Value + "%";
            }
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public override int GetHashCode() {
            return (int)Value;
        }

        public override bool Equals(object obj) {
            var other = obj as ElementSize;
            if (other == null)
                return false;

            return Math.Abs(other.Value - Value) < 0.00001 && other.Unit == Unit;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets element size value.
        /// </summary>
        public float Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        /// <summary>
        /// Gets or sets element size in pixels.
        /// </summary>
        public float Pixels
        {
            get
            {
                return _unit == ElementSizeUnit.Pixels
                    ? _value
                    : 0f;
            }
        }

        /// <summary>
        /// Gets or sets element size in percents.
        /// </summary>
        public float Percent
        {
            get
            {
                return _unit == ElementSizeUnit.Percents
                    ? _value
                    : 0f;
            }
        }
        
        /// <summary>
        /// Gets or sets element size unit.
        /// </summary>
        public ElementSizeUnit Unit
        {
            get
            {
                return _unit;
            }
            set
            {
                _unit = value;
            }
        }

        /// <summary>
        /// Gets or sets boolean indicating if element size is to fill out remaining space (used by DataGrid).
        /// </summary>
        public bool Fill
        {
            get
            {
                return _fill;
            }
            set
            {
                _fill = value;
            }
        }

        #endregion
    }
}
