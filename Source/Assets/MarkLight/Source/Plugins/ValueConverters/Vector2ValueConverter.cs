using System;
using UnityEngine;
using Marklight;

namespace MarkLight.ValueConverters
{
    /// <summary>
    /// Value converter for Vector2 type.
    /// </summary>
    public class Vector2ValueConverter : ValueConverter
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Vector2ValueConverter()
        {
            _type = typeof(Vector2);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Value converter for Vector2 type.
        /// </summary>
        public override ConversionResult Convert(object value, ValueConverterContext context)
        {
            if (value == null)
                return base.Convert(null, context);

            var valueType = value.GetType();
            if (valueType == _type)
                return base.Convert(value, context);

            var stringValue = value as string;
            if (stringValue == null)
                return ConversionFailed(value);

            try
            {
                var values = new ParsedNumber[2];
                var size = ParseUtils.ParseDelimitedNumbers(stringValue, values, -1, -1, context.ParseBuffer);

                switch (size)
                {
                    case 1:
                        var xy = values[0].NumberAsFloat;
                        return new ConversionResult(new Vector2(xy, xy));
                    case 2:
                        var x = values[0].NumberAsFloat;
                        var y = values[1].NumberAsFloat;
                        return new ConversionResult(new Vector2(x, y));
                    default:
                        return StringConversionFailed(value);
                }
            }
            catch (Exception e)
            {
                return ConversionFailed(value, e);
            }
        }

        /// <summary>
        /// Converts value to string.
        /// </summary>
        public override string ConvertToString(object value)
        {
            var v = (Vector2)value;
            return String.Format("{0},{1}", v.x, v.y);
        }

        #endregion
    }
}
