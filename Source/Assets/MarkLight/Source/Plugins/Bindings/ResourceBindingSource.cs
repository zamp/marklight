using System;

namespace MarkLight
{
    /// <summary>
    /// Resource binding source.
    /// </summary>
    public class ResourceBindingSource : BindingSource
    {
        #region Fields

        public readonly string DictionaryName;
        public readonly string ResourceKey;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ResourceBindingSource(string dictionaryName, string resourceKey)
        {
            DictionaryName = dictionaryName;
            ResourceKey = resourceKey;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets value from binding source.
        /// </summary>
        public override object GetValue(out bool hasValue)
        {
            return ResourceDictionary.GetValue(DictionaryName, ResourceKey, out hasValue);
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
                return String.IsNullOrEmpty(DictionaryName)
                    ? String.Format("@{0}", ResourceKey)
                    : String.Format("@{0}.{1}", DictionaryName, ResourceKey);
            }
        }

        #endregion
    }
}
