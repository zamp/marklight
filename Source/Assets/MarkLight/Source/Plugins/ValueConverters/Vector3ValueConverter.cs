using System;
using UnityEngine;
using Marklight;

namespace MarkLight.ValueConverters
{
    /// <summary>
    /// Value converter for Vector3 type.
    /// </summary>
    public class Vector3ValueConverter : ValueConverter
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Vector3ValueConverter()
        {
            _type = typeof(Vector3);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Value converter for Vector3 type.
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
                var values = new ParsedNumber[3];
                var size = ParseUtils.ParseDelimitedNumbers(stringValue, values, -1, -1, context.ParseBuffer);

                switch (size)
                {
                    case 1:
                        var xyz = values[0].NumberAsFloat;
                        return new ConversionResult(new Vector3(xyz, xyz, xyz));
                    case 2:
                        return new ConversionResult(new Vector3(
                            values[0].NumberAsFloat,
                            values[1].NumberAsFloat));
                    case 3:
                        return new ConversionResult(new Vector3(
                            values[0].NumberAsFloat,
                            values[1].NumberAsFloat,
                            values[2].NumberAsFloat));
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
            var v = (Vector3)value;
            return String.Format("{0},{1},{2}", v.x, v.y, v.z);
        }

        #endregion
    }
}
