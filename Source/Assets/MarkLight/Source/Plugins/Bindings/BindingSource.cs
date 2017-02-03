
namespace MarkLight
{
    /// <summary>
    /// Binding source.
    /// </summary>
    public abstract class BindingSource
    {
        #region Methods

        /// <summary>
        /// Gets value from binding source.
        /// </summary>
        public abstract object GetValue(out bool hasValue);

        #endregion

        #region Properties

        /// <summary>
        /// Gets binding source string.
        /// </summary>
        public abstract string BindingSourceString { get; }

        #endregion
    }
}
