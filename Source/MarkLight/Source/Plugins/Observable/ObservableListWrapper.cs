using System;
using System.Collections;
using System.Collections.Generic;

namespace MarkLight
{
    public class ObservableListWrapper<T> : IList<T>, IList
    {
        private readonly object _syncRoot = new object();
        private ObservableList<T> _list;

        public ObservableListWrapper(ObservableList<T> list)
        {
            _list = list;
        }

        public ObservableList<T> InnerList
        {
            get { return _list; }
            set { _list = value; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IObservableItem> Add(IEnumerable items)
        {
            throw new NotImplementedException();
        }

        public void Remove(IEnumerable items)
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            return _list.Contains((T)value);
        }

        void IList.Clear()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            return _list.IndexOf((T)value);
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        void IList.RemoveAt(int index) {
            throw new NotImplementedException();
        }

        object IList.this[int index]
        {
            get { return _list[index]; }
            set { throw new NotImplementedException();}
        }

        bool IList.IsReadOnly
        {
            get { return true; }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        void ICollection<T>.Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            _list.CopyTo(array as T[], index);
        }

        int ICollection.Count
        {
            get { return _list.Count; }
        }

        public object SyncRoot
        {
            get { return _syncRoot; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        int ICollection<T>.Count
        {
            get { return _list.Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return true; }
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public T this[int index]
        {
            get { return _list[index]; }
            set { throw new NotImplementedException(); }
        }
    }
}