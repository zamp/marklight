using System;
using MarkLight.Views.UI;

namespace MarkLight
{
    /// <summary>
    /// Event arguments for when a single data item is selected or deselected.
    /// </summary>
    public class DataItemSelectChangedEventArgs : EventArgs
    {
        #region Fields

        /// <summary>
        /// The index position of the data item that was selected.
        /// </summary>
        public readonly int Index;

        /// <summary>
        /// The data items selection state.
        /// </summary>
        public readonly bool IsSelected;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataItemSelectChangedEventArgs(int index, bool isSelected) {
            Index = index;
            IsSelected = isSelected;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the action change data.
        /// </summary>
        public ListChangedActionData ActionData
        {
            get { return new ListChangedActionData(ListChangeAction.Select, Index, Index); }
        }

        #endregion
    }
}