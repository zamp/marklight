using System;
using System.Globalization;
using Marklight;

namespace MarkLight.ValueConverters
{
    /// <summary>
    /// Value converter for int type.
    /// </summary>
    public class MarginValueConverter : ValueConverter
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public MarginValueConverter()
        {
            _type = typeof(ElementMargin);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Value converter for Margin type.
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
                    var values = new ParsedNumber[4];
                    var size = ParseUtils.ParseDelimitedNumbers(stringValue, values, -1, -1, context.ParseBuffer);

                    ElementMargin result;
                    switch (size)
                    {
                        case 1:
                            result = new ElementMargin(
                                values[0].ToElementSize(context.UnitSize));
                            break;
                        case 2:
                            result = new ElementMargin(
                                values[0].ToElementSize(context.UnitSize),
                                values[1].ToElementSize(context.UnitSize));
                            break;
                        case 3:
                            result = new ElementMargin(
                                values[0].ToElementSize(context.UnitSize),
                                values[1].ToElementSize(context.UnitSize),
                                values[2].ToElementSize(context.UnitSize));
                            break;
                        case 4:
                            result = new ElementMargin(
                                values[0].ToElementSize(context.UnitSize),
                                values[1].ToElementSize(context.UnitSize),
                                values[2].ToElementSize(context.UnitSize),
                                values[3].ToElementSize(context.UnitSize));
                            break;
                        default:
                            return StringConversionFailed(value);
                    }

                    return new ConversionResult(result);
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
                return new ConversionResult(new ElementMargin(convertedValue));
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
            var margin = (ElementMargin)value;
            return margin.ToString();
        }

        #endregion
    }
}
