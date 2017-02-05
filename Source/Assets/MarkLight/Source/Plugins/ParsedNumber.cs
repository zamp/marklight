using System;
using MarkLight;
using UnityEngine;

namespace Marklight
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
        public float NumberAsFloat {
            get { return String.IsNullOrEmpty(Number) ? 0f : float.Parse(Number); }
        }

        /// <summary>
        /// Convert the parsed value into an element size.
        /// </summary>
        public ElementSize ToElementSize(Vector3 unitSize) {

            var number = NumberAsFloat;
            if (String.IsNullOrEmpty(Unit) || Math.Abs(number) < 0.000001f)
                return new ElementSize(number);

            if (Unit == "*")
                return new ElementSize(1f, ElementSizeUnit.Percents) { Fill = true };

            if (Unit == "%")
                return new ElementSize(number / 100f, ElementSizeUnit.Percents);

            if (String.Equals(Unit, "px", StringComparison.OrdinalIgnoreCase))
                return new ElementSize(number);

            if (String.Equals(Unit, "ux", StringComparison.OrdinalIgnoreCase))
                return new ElementSize(number * unitSize.x);

            if (String.Equals(Unit, "uy", StringComparison.OrdinalIgnoreCase))
                return new ElementSize(number * unitSize.y);

            if (String.Equals(Unit, "uz", StringComparison.OrdinalIgnoreCase))
                return new ElementSize(number * unitSize.z);

            return new ElementSize(number);
        }
    }
}