using System;

namespace MarkLight
{
    [Serializable]
    public class ObservableWorkingListField : ViewField<ObservableList<object>>
    {
        [NonSerialized]
        private readonly ObservableList<object> _list = new ObservableList<object>();

        public override ObservableList<object> Value
        {
            get { return _list; }
            set { throw new NotImplementedException("Not allowed to set working list."); }
        }

        public override ObservableList<object> DirectValue
        {
            set { throw new NotImplementedException("Not allowed to set working list."); }
        }

        public override object ObjectValue
        {
            get { return _list; }
            set { throw new NotImplementedException("Not allowed to set working list."); }
        }

        public override object DirectObjectValue
        {
            set { throw new NotImplementedException("Not allowed to set working list."); }
        }

        public override ObservableList<object> InternalValue
        {
            get { return _list; }
            set { throw new NotImplementedException("Not allowed to set working list."); }
        }
    }
}