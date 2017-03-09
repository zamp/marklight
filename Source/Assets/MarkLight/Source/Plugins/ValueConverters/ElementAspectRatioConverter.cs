using System;
using UnityEngine;

namespace MarkLight.ValueConverters
{
    /// <summary>
    /// Value converter for ElementAspectRatio type.
    /// </summary>
    public class ElementAspectRatioConverter : ValueConverter
    {
        #region Fields

        private static readonly Type _intType = typeof(int);
        private static readonly Type _floatType = typeof(float);
        private static readonly Type _vector2Type = typeof(Vector2);

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ElementAspectRatioConverter()
        {
            _type = typeof(ElementAspectRatio);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Value converter for ElementAspectRatio type.
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
                try
                {
                    var result = ElementAspectRatio.Parse(stringValue);
                    return new ConversionResult(result);
                }
                catch (Exception e)
                {
                    return ConversionFailed(value, e);
                }
            }

            if (valueType == _intType || valueType == _floatType)
                return new ConversionResult(new ElementAspectRatio((float)value));

            if (valueType == _vector2Type)
            {
                var vec = (Vector2) value;
                return new ConversionResult(new ElementAspectRatio(vec.x, vec.y));
            }

            return ConversionFailed(value);
        }

        /// <summary>
        /// Converts value to string.
        /// </summary>
        public override string ConvertToString(object value)
        {
            var elementSize = (ElementAspectRatio)value;
            return elementSize.ToString();
        }

        #endregion
    }
}