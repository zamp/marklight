using System;

namespace MarkLight
{
    /// <summary>
    /// View field binding source.
    /// </summary>
    public class ViewFieldBindingSource : BindingSource
    {
        #region Fields

        public readonly ViewFieldData ViewFieldData;
        public readonly bool IsValueNegated;

        private static readonly string BoolTypeName = typeof(bool).Name;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ViewFieldBindingSource(ViewFieldData viewFieldData, bool isValueNegated = false)
        {
            ViewFieldData = viewFieldData;
            IsValueNegated = isValueNegated;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets value from binding source.
        /// </summary>
        public override object GetValue(out bool hasValue)
        {
            var value = ViewFieldData.GetValue(out hasValue);

            // check if value is to be negated
            if (IsValueNegated && ViewFieldData.TypeName == BoolTypeName)
            {
                value = !(bool)value;
            }

            return value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets binding source string.
        /// </summary>
        public override string BindingSourceString
        {
            get
            {
                return String.Format("{0}.{1}", ViewFieldData.SourceView.ViewTypeName, ViewFieldData.Path);
            }
        }

        #endregion
    }
}
