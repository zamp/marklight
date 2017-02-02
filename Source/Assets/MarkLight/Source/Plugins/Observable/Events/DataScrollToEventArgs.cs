using System;
using MarkLight.Views.UI;

namespace MarkLight
{
    /// <summary>
    /// Event arguments for when a view is scrolled to a data item.
    /// </summary>
    public class DataScrollToEventArgs : EventArgs
    {
        #region Fields

        /// <summary>
        /// Get the index position of the data item.
        /// </summary>
        public readonly int Index;

        /// <summary>
        /// Get the scroll alignment.
        /// </summary>
        public readonly ElementAlignment Alignment;

        /// <summary>
        /// Get the scroll offset.
        /// </summary>
        public readonly ElementMargin Offset;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataScrollToEventArgs(int index, ElementAlignment alignment, ElementMargin offset) {
            Index = index;
            Alignment = alignment;
            Offset = offset;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the action data.
        /// </summary>
        public ListChangedActionData ActionData
        {
            get { return new ListChangedActionData(ListChangeAction.ScrollTo, Index, Index); }
        }

        #endregion
    }
}