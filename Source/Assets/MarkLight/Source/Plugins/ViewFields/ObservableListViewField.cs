using System;

namespace MarkLight
{
    /// <summary>
    /// View field for types that implement IObservableList. Ensures modifications and selection changes
    /// triggers notification of dependent value observers on the owning view.
    /// </summary>
    [Serializable]
    public class ObservableListViewField<T> : ViewField<T> where T : IObservableList
    {

        #region Methods

        private void OnChanged(IObservableList old, IObservableList list)
        {
            if (old != null)
            {
                old.ItemsModified -= OnItemsModified;
                old.ItemSelectChanged -= OnItemSelectChanged;
            }

            if (list != null)
            {
                list.ItemsModified += OnItemsModified;
                list.ItemSelectChanged += OnItemSelectChanged;
            }
        }

        private void OnItemSelectChanged(object sender, DataItemSelectChangedEventArgs e)
        {
            OwnerView.Fields.NotifyDependentValueObservers(Path, true);
        }

        private void OnItemsModified(object sender, DataItemsModifiedEventArgs e)
        {
            OwnerView.Fields.NotifyDependentValueObservers(Path, true);
        }

        #endregion

        #region Properties

        public override T Value
        {
            get { return base.Value; }
            set
            {
                var oldList = base.Value;
                base.Value = value;
                OnChanged(oldList, value);
            }
        }

        public override T DirectValue
        {
            set
            {
                var oldList = base.Value;
                base.DirectValue = value;
                OnChanged(oldList, value);
            }
        }

        public override object ObjectValue
        {
            get { return base.ObjectValue; }
            set
            {
                var oldList = base.Value;
                base.ObjectValue = value;
                OnChanged(oldList, (IObservableList)value);
            }
        }

        public override object DirectObjectValue
        {
            set
            {
                var oldList = base.Value;
                base.DirectObjectValue = value;
                OnChanged(oldList, (IObservableList)value);
            }
        }

        #endregion
    }
}