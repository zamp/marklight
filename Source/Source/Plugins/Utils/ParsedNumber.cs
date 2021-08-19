using System;
using UnityEngine;

namespace MarkLight
{
    /// <summary>
    /// Holds parsed value split into 2 parts. The "Number" which is the initial text comprised of digits only,
    /// and the "Suffix" which are alpha-numeric characters that suffix the initial numbers.
    /// </summary>
    public struct ParsedNumber
    {
        /// <summary>
        /// Initial number characters.
        /// </summary>
        public string Number;

        /// <summary>
        /// Suffix to the number characters.
        /// </summary>
        public string Unit;

        /// <summary>
        /// Parse the Number field into a float value.
        /// </summary>
        public float NumberAsFloat
        {
            get { return String.IsNullOrEmpty(Number) ? 0f : float.Parse(Number); }
        }

        /// <summary>
        /// Convert the parsed value into an element size.
        /// </summary>
        public ElementSize ToElementSize(Vector3 unitSize)
        {
            if (Unit == "*")
                return ElementSize.FromPercents(1f, true);

            var number = NumberAsFloat;
            if (String.IsNullOrEmpty(Unit) || Math.Abs(number) < 0.000001f)
                return ElementSize.FromPixels(number);

            if (Unit == "%")
                return ElementSize.FromPercents(number / 100f);

            if (String.Equals(Unit, "px", StringComparison.OrdinalIgnoreCase))
                return ElementSize.FromPixels(number);

            if (String.Equals(Unit, "ux", StringComparison.OrdinalIgnoreCase))
                return ElementSize.FromPixels(number * unitSize.x);

            if (String.Equals(Unit, "uy", StringComparison.OrdinalIgnoreCase))
                return ElementSize.FromPixels(number * unitSize.y);

            if (String.Equals(Unit, "uz", StringComparison.OrdinalIgnoreCase))
                return ElementSize.FromPixels(number * unitSize.z);

            return new ElementSize(number);
        }
    }
}