namespace MarkLight
{
    /// <summary>
    /// Event arguments for when one or more data model items are modfied.
    /// </summary>
    public class DataItemsModifiedEventArgs : DataItemsEventArgs
    {
        #region Fields

        /// <summary>
        /// Path to modified field within data model item.
        /// </summary>
        public readonly string FieldPath;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataItemsModifiedEventArgs(int startIndex, int count, string fieldPath) : base(startIndex, count) {
            FieldPath = fieldPath;
        }

        #endregion

        #region Properties

        public override ListChangeAction Action
        {
            get { return ListChangeAction.Modify; }
        }

        #endregion
    }
}