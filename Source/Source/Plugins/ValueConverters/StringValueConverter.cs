using System;
using System.Globalization;

namespace MarkLight.ValueConverters
{
    /// <summary>
    /// Value converter for String type.
    /// </summary>
    public class StringValueConverter : ValueConverter
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public StringValueConverter()
        {
            _type = typeof(string);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Value converter for String type.
        /// </summary>
        public override ConversionResult Convert(object value, ValueConverterContext context)
        {
            if (value == null)
                return base.Convert(null, context);

            var stringValue = value as string;
            if (stringValue != null)
                return new ConversionResult(stringValue)
                    ;
            // attempt to convert using system type converter
            try
            {
                var convertedValue = System.Convert.ToString(value, CultureInfo.InvariantCulture);
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
            return (string)value;
        }

        #endregion
    }
}
