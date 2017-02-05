using System;
using Marklight;

namespace MarkLight.ValueConverters
{
    /// <summary>
    /// Value converter for enum types.
    /// </summary>
    public class EnumValueConverter : ValueConverter
    {
        #region Fields

        public static readonly EnumValueConverter HexColorType = new EnumValueConverter(typeof(HexColorType));

        private readonly Type _enumType;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public EnumValueConverter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public EnumValueConverter(Type enumType)
        {
            _enumType = enumType;
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

            var valueType = value.GetType();
            if (valueType == _enumType)
                return base.Convert(value, context);

            var stringValue = value as string;
            if (stringValue == null)
                return StringConversionFailed(value);

            try
            {
                var convertedValue = Enum.Parse(_enumType, stringValue, true);
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
            return Enum.GetName(_enumType, value);
        }

        #endregion
    }
}
