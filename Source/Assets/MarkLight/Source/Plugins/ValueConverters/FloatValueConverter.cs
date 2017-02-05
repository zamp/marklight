using System;
using System.Globalization;

namespace MarkLight.ValueConverters
{
    /// <summary>
    /// Value converter for float type.
    /// </summary>
    public class FloatValueConverter : ValueConverter
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public FloatValueConverter()
        {
            _type = typeof(float);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Value converter for float type.
        /// </summary>
        public override ConversionResult Convert(object value, ValueConverterContext context)
        {
            if (value == null)
                return base.Convert(null, context);

            var valueType = value.GetType();
            if (valueType == _type)
            {
                return base.Convert(value, context);
            }

            var stringValue = value as string;
            if (stringValue != null)
            {
                try
                {
                    var convertedValue = System.Convert.ToSingle(stringValue, CultureInfo.InvariantCulture);
                    return new ConversionResult(convertedValue);
                }
                catch (Exception e)
                {
                    return ConversionFailed(value, e);
                }
            }

            // attempt to convert using system type converter
            try
            {
                var convertedValue = System.Convert.ToSingle(value, CultureInfo.InvariantCulture);
                return new ConversionResult(convertedValue);
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
            return value.ToString();
        }

        #endregion
    }
}
