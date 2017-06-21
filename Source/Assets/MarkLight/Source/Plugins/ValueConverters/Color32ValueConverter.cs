using System;
using UnityEngine;

namespace MarkLight.ValueConverters
{
    /// <summary>
    /// Value converter for Color type.
    /// </summary>
    public class Color32ValueConverter : ValueConverter
    {
        #region Fields

        protected ColorValueConverter _colorValueConverter;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Color32ValueConverter()
        {
            _type = typeof(Color32);
            _colorValueConverter = ColorValueConverter.Instance;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Value converter for Color32 type.
        /// </summary>
        public override ConversionResult Convert(object value, ValueConverterContext context)
        {
            if (value == null)
                return base.Convert(null, context);

            var valueType = value.GetType();
            if (valueType == _type)
                return base.Convert(value, context);

            var result = _colorValueConverter.Convert(value, context);
            if (result.Success)
                result.ConvertedValue = (Color32)(Color)result.ConvertedValue;

            return result;
        }

        /// <summary>
        /// Converts value to string.
        /// </summary>
        public override string ConvertToString(object value)
        {
            return _colorValueConverter.ConvertToString((Color)(Color32)value);
        }

        #endregion
    }
}
