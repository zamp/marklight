using System;
using System.Globalization;

namespace MarkLight.ValueConverters
{
    /// <summary>
    /// Value converter for int type.
    /// </summary>
    public class IntValueConverter : ValueConverter
    {
        #region Fields

        public static readonly IntValueConverter Instance = new IntValueConverter();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public IntValueConverter()
        {
            _type = typeof(int);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Value converter for int type.
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
                    var convertedValue = System.Convert.ToInt32(stringValue, CultureInfo.InvariantCulture);
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
                var convertedValue = System.Convert.ToInt32(value, CultureInfo.InvariantCulture);
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
