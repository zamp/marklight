#region Using Statements
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

namespace MarkLight
{
    /// <summary>
    /// Interface for lists that notify observers when items are added, deleted or moved.
    /// </summary>
    public interface IObservableList
    {
        #region Fields

        event EventHandler<DataItemsAddedEventArgs> ItemsAdded;
        event EventHandler<DataItemsRemovedEventArgs> ItemsRemoved;
        event EventHandler<DataItemsModifiedEventArgs> ItemsModified;
        event EventHandler<DataItemsMovedEventArgs> ItemsMoved;
        event EventHandler<DataItemSelectChangedEventArgs> ItemSelectChanged;
        event EventHandler<DataScrollToEventArgs> ScrolledTo;

        #endregion

        #region Methods

        /// <summary>
        /// Determine if the list contains a specified data item.
        /// </summary>
        bool Contains(object item);

        /// <summary>
        /// Add a data item to the list and get its observable item in return.
        /// </summary>
        IObservableItem Add(object item);

        /// <summary>
        /// Add multiple data items to the list and get their observable items in return.
        /// </summary>
        IEnumerable<IObservableItem> Add(IEnumerable items);

        /// <summary>
        /// Remove a data item from the list.
        /// </summary>
        bool Remove(object item);

        /// <summary>
        /// Remove multiple data items from the list.
        /// </summary>
        void Remove(IEnumerable items);

        /// <summary>
        /// Clear all data items from the list.
        /// </summary>
        void Clear();

        /// <summary>
        /// Get the index position of a data item in the list. -1 if not found.
        /// </summary>
        int GetIndex(object item);

        /// <summary>
        /// Get a data items observable item. Null if the data item was not found in the list.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        IObservableItem GetObservable(object item);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of items in the list.
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// Get list as IObservableItem read only collection.
        /// </summary>
        IList<IObservableItem> Observables
        {
            get;
        }

        /// <summary>
        /// Get list of values as read only collection.
        /// </summary>
        IList<object> Values
        {
            get;
        }

        /// <summary>
        /// Get or set the selected item.
        /// </summary>
        object SelectedItem
        {
            get; set;
        }

        /// <summary>
        /// Get or set the selected observable item.
        /// </summary>
        IObservableItem SelectedObservable
        {
            get; set;
        }

        /// <summary>
        /// Get the selected index.
        /// </summary>
        int SelectedIndex
        {
            get; set;
        }

        /// <summary>
        /// Get a readonly list of selected items
        /// </summary>
        ICollection<object> SelectedItems
        {
            get;
        }

        /// <summary>
        /// Get a readonly list of selected observable items
        /// </summary>
        ICollection<IObservableItem> SelectedObservables
        {
            get;
        }

        /// <summary>
        /// Gets the item at index.
        /// </summary>
        object this[int index] { get; }

        /// <summary>
        /// Get the last scroll event produced.
        /// </summary>
        DataScrollToEventArgs LastScroll { get; }

        #endregion
    }
}
