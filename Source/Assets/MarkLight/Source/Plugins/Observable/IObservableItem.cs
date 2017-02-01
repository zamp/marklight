using System;

namespace MarkLight
{
    /// <summary>
    /// Interface for observable list items that are individually observed.
    /// </summary>
    public interface IObservableItem
    {
        event EventHandler<DataItemSelectChangedEventArgs> ItemSelectChanged;
        event EventHandler<DataItemIndexChangedEventArgs> ItemIndexChanged;
        event EventHandler<DataItemsModifiedEventArgs> ItemModified;

        /// <summary>
        /// Get the parent data model.
        /// </summary>
        IObservableList DataModel { get; }

        /// <summary>
        /// Get the index position of the item in the data model.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Get the data model item.
        /// </summary>
        object Value { get; }

        /// <summary>
        /// Get or set the selected state of the item. Setting the same value as the current
        /// value is ignored.
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Change the selected state of the item and force all events if the current state
        /// is the same as the new state.
        /// </summary>
        void ForceSelected(bool isSelected);

        /// <summary>
        /// Notify the model that the item has been modfied..
        /// </summary>
        void NotifyModified();

        /// <summary>
        /// Scroll to the item in the view.
        /// </summary>
        void ScrollTo(ElementAlignment? alignment = null, ElementMargin offset = null);
    }
}