using System;
using UnityEngine;

namespace MarkLight.ValueConverters
{
    /// <summary>
    /// Value converter for component types.
    /// </summary>
    public class ComponentValueConverter : ValueConverter
    {
        #region Fields 

        private readonly Type _componentType;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ComponentValueConverter()
        {
            _type = typeof(Component);
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ComponentValueConverter(Type componentType)
        {
            _type = typeof(Component);
            _componentType = componentType;
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
            if (valueType.IsSubclassOf(_type))
                return base.Convert(value, context);

            var stringValue = value as string;
            if (stringValue == null)
                return ConversionFailed(value);

            try
            {
                var go = GameObject.Find(stringValue);
                var component = go.GetComponent(_componentType);
                return component == null
                    ? ConversionFailed(value)
                    : new ConversionResult(component);
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
            var component = (Component)value;
            return component.gameObject.name;
        }

        #endregion
    }
}
