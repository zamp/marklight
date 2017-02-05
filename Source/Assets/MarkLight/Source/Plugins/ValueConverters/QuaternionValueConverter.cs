using System;
using UnityEngine;

namespace MarkLight.ValueConverters
{
    /// <summary>
    /// Value converter for Quaternion type.
    /// </summary>
    public class QuaternionValueConverter : ValueConverter
    {
        #region Fields

        private static readonly Type _vector3Type = typeof(Vector3);
        private readonly Vector3ValueConverter _vector3ValueConverter;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public QuaternionValueConverter()
        {
            _type = typeof(Quaternion);
            _vector3ValueConverter = new Vector3ValueConverter();
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
            if (stringValue != null)
            {
                var result = _vector3ValueConverter.Convert(stringValue, context);
                return result.Success
                    ? new ConversionResult(Quaternion.Euler((Vector3)result.ConvertedValue))
                    : StringConversionFailed(stringValue);
            }

            return valueType == _vector3Type
                ? new ConversionResult(Quaternion.Euler((Vector3)value))
                : ConversionFailed(value);
        }

        /// <summary>
        /// Converts value to string.
        /// </summary>
        public override string ConvertToString(object value)
        {
            var quaternion = (Quaternion)value;
            var eulerAngles = quaternion.eulerAngles;
            return String.Format("{0},{1},{2}", eulerAngles.x, eulerAngles.y, eulerAngles.z);
        }

        #endregion
    }
}
