using System;

namespace MarkLight.ValueConverters
{
    /// <summary>
    /// Value converter for ElementSize type.
    /// </summary>
    public class ElementSizeValueConverter : ValueConverter
    {
        #region Fields

        private static readonly Type _intType = typeof(int);
        private static readonly Type _floatType = typeof(float);
        
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ElementSizeValueConverter()
        {
            _type = typeof(ElementSize);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Value converter for ElementSize type.
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
                    var result = ElementSize.Parse(stringValue, context.UnitSize, context.ParseBuffer);
                    return new ConversionResult(result);
                }
                catch (Exception e)
                {
                    return ConversionFailed(value, e);
                }
            }

            if (valueType == _intType)
                return new ConversionResult(ElementSize.FromPixels((float)value));

            if (valueType == _floatType)
                return new ConversionResult(ElementSize.FromPixels((float)value));

            return ConversionFailed(value);
        }

        /// <summary>
        /// Converts value to string.
        /// </summary>
        public override string ConvertToString(object value)
        {
            var elementSize = (ElementSize)value;
            return elementSize.ToString();
        }

        #endregion
    }
}
