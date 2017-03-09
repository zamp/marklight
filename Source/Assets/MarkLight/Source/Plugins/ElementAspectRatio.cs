using System;
using System.Globalization;
using UnityEngine;

namespace MarkLight
{
    /// <summary>
    /// Represents the desired aspect ratio to maintain.
    /// </summary>
    [Serializable]
    public struct ElementAspectRatio
    {
        #region Fields

        private static readonly char[] StringDelimiters = new char[] {' ', ':', ',', '\t'};

        [SerializeField]
        private float _x;

        [SerializeField]
        private float _y;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="aspectValue">The aspect ratio</param>
        public ElementAspectRatio(float aspectValue = 1.0f) : this()
        {
            _x = aspectValue;
            _y = 1.0f;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="aspectX">The aspect X component (width)</param>
        /// <param name="aspectY">The aspect Y component (height)</param>
        public ElementAspectRatio(float aspectX, float aspectY) : this()
        {
            _x = aspectX;
            _y = aspectY;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return X.ToString(CultureInfo.InvariantCulture) + ':' + Y.ToString(CultureInfo.InvariantCulture);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ElementAspectRatio))
                return false;

            var other = (ElementAspectRatio) obj;
            return Math.Abs(X - other.X) < 0.00001f && Math.Abs(Y - other.Y) < 0.00001f;
        }

        /// <summary>
        /// Parses string into element size.
        /// </summary>
        public static ElementAspectRatio Parse(string value)
        {
            value = value.Trim();

            var components = value.Split(StringDelimiters, StringSplitOptions.RemoveEmptyEntries);

            if (components.Length > 1)
            {
                var x = float.Parse(components[0]);
                var y = float.Parse(components[1]);
                return new ElementAspectRatio(x, y);
            }
            if (components.Length == 1)
            {
                var ratio = float.Parse(components[0]);
                return new ElementAspectRatio(ratio);
            }

            return new ElementAspectRatio(1f);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the ratio value which is X / Y;
        /// </summary>
        public float Value
        {
            get { return X / Y; }
        }

        /// <summary>
        /// Aspect ratio X component (width).
        /// </summary>
        public float X
        {
            get { return _x; }
        }

        /// <summary>
        /// Aspect ratio Y compoennt (height).
        /// </summary>
        public float Y
        {
            get { return _y; }
        }

        #endregion
    }
}