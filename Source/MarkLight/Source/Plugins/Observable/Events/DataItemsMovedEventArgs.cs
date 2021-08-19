using System;
using MarkLight.Views.UI;

namespace MarkLight
{
    /// <summary>
    /// Event arguments for when index position of data model items changes.
    /// </summary>
    public class DataItemsMovedEventArgs : DataItemsEventArgs
    {
        #region Fields

        /// <summary>
        /// Reason for the data model item(s) being moved.
        /// </summary>
        public readonly DataItemsMoveReason MoveReason;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataItemsMovedEventArgs(DataItemsMoveReason reason, int startIndex, int count) : base(startIndex, count) {
            MoveReason = reason;
        }

        #endregion

        #region Properties

        public override ListChangeAction Action
        {
            get { return ListChangeAction.Move; }
        }

        #endregion
    }

    public enum DataItemsMoveReason
    {
        Sort,
        Reverse
    }
}