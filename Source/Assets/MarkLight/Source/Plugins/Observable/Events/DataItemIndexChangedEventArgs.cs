using System;

namespace MarkLight
{
    /// <summary>
    /// Event arguments for when a data model item's index position is changed.
    /// </summary>
    public class DataItemIndexChangedEventArgs : EventArgs
    {
        #region Fields

        /// <summary>
        /// The data item's old index position.
        /// </summary>
        public readonly int OldIndex;

        /// <summary>
        /// The data item's new and current index position.
        /// </summary>
        public readonly int NewIndex;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataItemIndexChangedEventArgs(int oldIndex, int newIndex) {
            OldIndex = oldIndex;
            NewIndex = newIndex;
        }

        #endregion
    }
}