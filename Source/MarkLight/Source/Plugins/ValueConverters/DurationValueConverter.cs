using System;
using System.Globalization;

namespace MarkLight.ValueConverters
{
    /// <summary>
    /// Value converter for duration.
    /// </summary>
    public class DurationValueConverter : ValueConverter
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public DurationValueConverter()
        {
            _type = typeof(DurationValueConverter);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Value converter for duration type.
        /// </summary>
        public override ConversionResult Convert(object value, ValueConverterContext context)
        {
            if (value == null)
                return base.Convert(null, context);

            var valueType = value.GetType();
            if (valueType == _type)
                return base.Convert(value, context);

            var str = value as string;
            if (str == null)
                return ConversionFailed(value);

            try
            {
                str = str.Trim();
                float duration;
                var endIndex = str.Length - 1;

                if (str.Length > 1 && str.LastIndexOf("s", StringComparison.OrdinalIgnoreCase) == endIndex)
                {
                    if (str.Length > 2 && str.LastIndexOf("m", StringComparison.OrdinalIgnoreCase) == endIndex - 1)
                    {
                        // milliseconds
                        duration =
                            System.Convert.ToSingle(
                                str.Substring(0, endIndex - 2), CultureInfo.InvariantCulture) / 1000f;
                    }
                    else
                    {
                        // seconds
                        duration = System.Convert.ToSingle(
                            str.Substring(0, endIndex - 1), CultureInfo.InvariantCulture);
                    }
                }
                else if (str.Length > 3 && str.LastIndexOf("min", StringComparison.OrdinalIgnoreCase) == endIndex - 3)
                {
                    // minutes
                    duration = System.Convert.ToSingle(
                                   str.Substring(0, endIndex - 4), CultureInfo.InvariantCulture) * 60f;
                }
                else
                {
                    // seconds
                    duration = System.Convert.ToSingle(str, CultureInfo.InvariantCulture);
                }

                return new ConversionResult(duration);
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
            return value + "s";
        }

        #endregion
    }
}
