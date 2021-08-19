using System;

namespace MarkLight
{
    /// <summary>
    /// View field for the Item field in views. Allows adding
    /// IObservableItem as data.
    /// </summary>
    [Serializable]
    public class ItemViewField : ViewField<object>
    {
        #region Methods

        public override void NotifyModified() {
            base.NotifyModified();
            var model = OwnerView.Bindings.Item;
            if (model != null)
                model.NotifyModified();
        }

        #endregion

        #region Properties

        public override object Value
        {
            get { return base.Value; }
            set
            {
                var observableItem = value as IObservableItem;
                OwnerView.Bindings.Item = observableItem;
                base.Value = observableItem != null
                    ? observableItem.Value
                    : value;
            }
        }

        public override object DirectValue
        {
            set
            {
                var observableItem = value as IObservableItem;
                OwnerView.Bindings.Item = observableItem;
                base.DirectValue = observableItem != null
                    ? observableItem.Value
                    : value;
            }
        }

        public override object ObjectValue
        {
            get { return base.ObjectValue; }
            set
            {
                var observableItem = value as IObservableItem;
                OwnerView.Bindings.Item = observableItem;
                base.ObjectValue = observableItem != null
                    ? observableItem.Value
                    : value;
            }
        }

        public override object DirectObjectValue
        {
            set
            {
                var observableItem = value as IObservableItem;
                OwnerView.Bindings.Item = observableItem;
                base.DirectObjectValue = observableItem != null
                    ? observableItem.Value
                    : value;
            }
        }

        #endregion
    }
}