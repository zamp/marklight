using System;
using MarkLight.Views.UI;

namespace MarkLight
{
    /// <summary>
    /// Abstract event arguments for data items events.
    /// </summary>
    public abstract class DataItemsEventArgs : EventArgs
    {
        #region Fields

        /// <summary>
        /// Get the start index position of the affected model data.
        /// </summary>
        public readonly int StartIndex;

        /// <summary>
        /// Get the end index position of the affected model data.
        /// </summary>
        public readonly int EndIndex;

        /// <summary>
        /// Get the number of model data items affected.
        /// </summary>
        public readonly int Count;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="startIndex">The start index position of the affected model data.</param>
        /// <param name="count">The number of model data items affected.</param>
        protected DataItemsEventArgs(int startIndex, int count) {
            StartIndex = startIndex;
            EndIndex = startIndex + count - 1;
            Count = count;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the list change action.
        /// </summary>
        public abstract ListChangeAction Action { get; }

        /// <summary>
        /// Get the event action data.
        /// </summary>
        public ListChangedActionData ActionData
        {
            get { return new ListChangedActionData(Action, StartIndex, EndIndex); }
        }

        #endregion
    }
}