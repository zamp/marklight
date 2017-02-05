using System;
using UnityEngine;
using Marklight;

namespace MarkLight.ValueConverters
{
    /// <summary>
    /// Value converter for Vector4 type.
    /// </summary>
    public class Vector4ValueConverter : ValueConverter
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Vector4ValueConverter()
        {
            _type = typeof(Vector4);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Value converter for Vector4 type.
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
                var values = new ParsedNumber[4];
                var size = ParseUtils.ParseDelimitedNumbers(stringValue, values, -1, -1, context.ParseBuffer);

                switch (size)
                {
                    case 1:
                        var xyzw = values[0].NumberAsFloat;
                        return new ConversionResult(new Vector4(xyzw, xyzw, xyzw, xyzw));
                    case 2:
                        return new ConversionResult(new Vector4(
                            values[0].NumberAsFloat,
                            values[1].NumberAsFloat));
                    case 3:
                        return new ConversionResult(new Vector4(
                            values[0].NumberAsFloat,
                            values[1].NumberAsFloat,
                            values[2].NumberAsFloat));
                    case 4:
                        return new ConversionResult(new Vector4(
                            values[0].NumberAsFloat,
                            values[1].NumberAsFloat,
                            values[2].NumberAsFloat,
                            values[3].NumberAsFloat));
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
            var v = (Vector4)value;
            return String.Format("{0},{1},{2},{3}", v.x, v.y, v.z, v.w);
        }

        #endregion
    }
}
