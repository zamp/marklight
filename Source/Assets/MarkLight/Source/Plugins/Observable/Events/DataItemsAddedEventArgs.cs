using System.Collections.Generic;

namespace MarkLight
{
    /// <summary>
    /// Event arguments for when one or more data model items are added.
    /// </summary>
    public class DataItemsAddedEventArgs : DataItemsEventArgs
    {
        #region Fields

        /// <summary>
        /// Reason the data is being added.
        /// </summary>
        public readonly DataItemAddReason AddReason;

        /// <summary>
        /// The added data items.
        /// </summary>
        public readonly IEnumerable<IObservableItem> Added;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataItemsAddedEventArgs(DataItemAddReason reason, int startIndex, int count,
                                            IEnumerable<IObservableItem> added) : base(startIndex, count) {
            AddReason = reason;
            Added = added;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataItemsAddedEventArgs(DataItemAddReason reason, IObservableItem added) : base (added.Index, 1) {
            AddReason = reason;
            Added = new List<IObservableItem>
            {
                added
            };
        }

        #endregion

        #region Properties

        public override ListChangeAction Action
        {
            get { return ListChangeAction.Add; }
        }

        #endregion
    }

    public enum DataItemAddReason {
        Add = 0,
        Insert = 1,
        Replace = 2
    }
}