
using System;

namespace MarkLight
{
    /// <summary>
    /// View field for the Item field in views. Allows adding
    /// IObservableItem as data.
    /// </summary>
    [Serializable]
    public class ItemDataViewField : ViewField<object>
    {
        [NonSerialized]
        private IObservableItem _observableItem;

        /// <summary>
        /// Get observable item, if any. Null if there is no
        /// observable item.
        /// </summary>
        public IObservableItem ObservableItem
        {
            get { return _observableItem; }
        }

        public override object Value
        {
            get { return base.Value; }
            set
            {
                _observableItem = value as IObservableItem;
                base.Value = ObservableItem != null
                    ? ObservableItem.Value
                    : value;
            }
        }

        public override object DirectValue
        {
            set {
                _observableItem = value as IObservableItem;
                base.DirectValue = ObservableItem != null
                    ? ObservableItem.Value
                    : value;
            }
        }

        public override object ObjectValue
        {
            get { return base.ObjectValue; }
            set
            {
                _observableItem = value as IObservableItem;
                base.ObjectValue = ObservableItem != null
                    ? ObservableItem.Value
                    : value;
            }
        }

        public override object DirectObjectValue
        {
            set
            {
                _observableItem = value as IObservableItem;
                base.DirectObjectValue = ObservableItem != null
                    ? ObservableItem.Value
                    : value;
            }
        }
    }
}