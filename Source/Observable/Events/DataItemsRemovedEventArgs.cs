using System.Collections.Generic;

namespace MarkLight
{
    /// <summary>
    /// Event arguments for when one or more data model items are removed.
    /// </summary>
    public class DataItemsRemovedEventArgs : DataItemsEventArgs
    {
        #region Fields

        /// <summary>
        /// The reason the data items were removed.
        /// </summary>
        public DataItemsRemovedReason RemoveReason;

        /// <summary>
        /// The items that were removed.
        /// </summary>
        public readonly IEnumerable<IObservableItem> Removed;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataItemsRemovedEventArgs(DataItemsRemovedReason reason, int startIndex, int count,
                                            IEnumerable<IObservableItem> removed) : base(startIndex, count)
        {
            RemoveReason = reason;
            Removed = removed;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataItemsRemovedEventArgs(DataItemsRemovedReason reason, IObservableItem removed)
                                                                                    : base(removed.Index, 1)
        {
            RemoveReason = reason;
            Removed = new List<IObservableItem>
            {
                removed
            };
        }

        #endregion

        #region Properties

        public override ListChangeAction Action
        {
            get {
                return RemoveReason == DataItemsRemovedReason.Clear
                    ? ListChangeAction.Clear
                    : ListChangeAction.Remove;
            }
        }

        #endregion
    }

    public enum DataItemsRemovedReason {
        Remove = 0,
        Clear = 1,
        Replace = 2
    }
}