#region Using Statements
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace MarkLight
{
    /// <summary>
    /// Generic list that notify observers when items are added, deleted or moved.
    /// </summary>
    [Serializable]
    public class ObservableList<T> : IObservableList, IList<T>
    {
        private static readonly IObservableItem[]  EmptyListItemArray = new IObservableItem[0];

        #region Fields

        private readonly List<IObservableItem> _list;
        private readonly List<IObservableItem> _selectedItems;
        private ObservableListWrapper<T> _readonlyList;
        private ListConverter<T> _tSelectedItems;
        private ListConverter<object> _objSelectedItems;
        private ListConverter<object> _values;
        private IObservableItem _selectedItem;

        public event EventHandler<DataItemsAddedEventArgs> ItemsAdded;
        public event EventHandler<DataItemsRemovedEventArgs> ItemsRemoved;
        public event EventHandler<DataItemsModifiedEventArgs> ItemsModified;
        public event EventHandler<DataItemsMovedEventArgs> ItemsMoved;
        public event EventHandler<DataItemSelectChangedEventArgs> ItemSelectChanged;
        public event EventHandler<DataScrollToEventArgs> ScrolledTo;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ObservableList()
        {
            _list = new List<IObservableItem>();
            _selectedItems = new List<IObservableItem>();
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ObservableList(int capacity)
        {
            _list = new List<IObservableItem>(capacity);
            _selectedItems = new List<IObservableItem>();
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ObservableList(IEnumerable<T> collection)
        {
            _list = new List<IObservableItem>();
            _selectedItems = new List<IObservableItem>();
            AddRange(collection);
        }

        #endregion

        #region Methods


        /// <summary>
        /// Adds item to the end of the list.
        /// </summary>
        public IObservableItem Add(T item) {

            if (item == null)
                throw new NullReferenceException("Cannot add null to ObservableList.");

            var index = _list.Count;
            var result = new ObservableItem(this, index, item);

            _list.Add(result);

            if (ItemsAdded != null)
                ItemsAdded(this, new DataItemsAddedEventArgs(DataItemAddReason.Add, result));

            return result;
        }

        /// <summary>
        /// Adds item to the end of the list.
        /// </summary>
        IObservableItem IObservableList.Add(object item) {

            if (item == null)
                throw new NullReferenceException("Cannot add null to ObservableList.");

            var observable = item as IObservableItem;
            return Add(observable == null ? (T)item : (T)observable.Value);
        }

        /// <summary>
        /// Adds item to the end of the list.
        /// </summary>
        void ICollection<T>.Add(T item) {
            Add(item);
        }

        /// <summary>
        /// Adds a range of items to the list.
        /// </summary>
        public IEnumerable<IObservableItem> Add(IEnumerable items) {
            var result = new List<IObservableItem>();
            foreach (var item in items)
            {
                if (item == null)
                    throw new NullReferenceException("Cannot add null to ObservableList.");

                var index = _list.Count;
                var observable = new ObservableItem(this, index, (T)item);

                _list.Add(observable);
                result.Add(observable);
            }

            if (ItemsAdded != null && result.Count > 0)
            {
                ItemsAdded(this,
                    new DataItemsAddedEventArgs(DataItemAddReason.Add, result[0].Index, result.Count, result));
            }

            return result;
        }

        /// <summary>
        /// Adds a range of items to the end of the list.
        /// </summary>
        public IEnumerable<IObservableItem> AddRange(IEnumerable<T> items)
        {
            var itemCount = items.Count();
            if (itemCount <= 0)
                return EmptyListItemArray;

            var startIndex = _list.Count;
            var endIndex = startIndex + (itemCount - 1);

            var newSize = _list.Count + itemCount;
            if (_list.Capacity < newSize)
                _list.Capacity = newSize;

            var result = new List<IObservableItem>(itemCount);

            foreach (var item in items)
            {
                if (item == null)
                    throw new NullReferenceException("Cannot add null to ObservableList.");

                var index = _list.Count;
                var observableItem = new ObservableItem(this, index, item);
                _list.Add(observableItem);
                result.Add(observableItem);
            }

            if (ItemsAdded != null && result.Count > 0)
            {
                ItemsAdded(this,
                    new DataItemsAddedEventArgs(DataItemAddReason.Add, result[0].Index, result.Count, result));
            }

            return result;
        }

        /// <summary>
        /// Replaces the items in the list.
        /// </summary>
        public IEnumerable<IObservableItem> Replace(IEnumerable<T> newItems) {

            var newItemsList = newItems as IList<T> ?? new List<T>(newItems);

            var itemCount = newItemsList.Count;
            if (itemCount <= 0)
            {
                Clear();
                return EmptyListItemArray;
            }

            var replaceCount = itemCount >= Count
                ? Count
                : itemCount;

            var replaced = new List<IObservableItem>(replaceCount);
            var result = new List<IObservableItem>(Math.Max(replaceCount, itemCount));

            for (var i = 0; i < replaceCount; ++i)
            {
                var item = newItemsList[i];
                if (item == null)
                    throw new NullReferenceException("Cannot add null to ObservableList.");

                var observableItem = new ObservableItem(this, i, item);

                var old = _list[i];
                old.IsSelected = false;
                ((ObservableItem)old).Dispose();
                replaced.Add(old);

                _list[i] = observableItem;
                result.Add(observableItem);
            }

            if (replaceCount > 0)
            {
                if (ItemsRemoved != null)
                {
                    ItemsRemoved(this,
                        new DataItemsRemovedEventArgs(DataItemsRemovedReason.Replace, 0, replaceCount, replaced));
                }

                if (ItemsAdded != null)
                {
                    ItemsAdded(this,
                        new DataItemsAddedEventArgs(DataItemAddReason.Replace, 0, replaceCount, result.AsReadOnly()));
                }
            }

            if (itemCount > Count)
            {
                // old list smaller than new - add items
                result.AddRange(AddRange(newItemsList.Skip(replaceCount)));
            }
            else if (itemCount < Count)
            {
                // old list larger than new - remove items
                RemoveRange(itemCount, Count - itemCount);
            }

            return result;
        }

        /// <summary>
        /// Replaces a single item in the list.
        /// </summary>
        public IObservableItem Replace(int index, T item)
        {
            if (item == null)
                throw new NullReferenceException("Cannot add null to ObservableList.");

            if (index < 0 || index >= Count)
                return null;

            var old = _list[index];

            if (ReferenceEquals(item, old.Value))
                return old;

            old.IsSelected = false;
            ((ObservableItem)old).Dispose();

            var result = new ObservableItem(this, index, item);
            _list[index] = result;

            if (ItemsRemoved != null)
                ItemsRemoved(this, new DataItemsRemovedEventArgs(DataItemsRemovedReason.Replace, old));

            if (ItemsAdded != null)
                ItemsAdded(this, new DataItemsAddedEventArgs(DataItemAddReason.Replace, result));

            return result;
        }

        /// <summary>
        /// Informs observers that item has been modified.
        /// </summary>
        public void NotifyItemModified(T item, string fieldPath = "")
        {
            int index = IndexOf(item);
            if (index < 0)
                return;

            NotifyItemsModified(index, index, fieldPath);
        }

        /// <summary>
        /// Informs observers that item has been modified.
        /// </summary>
        public void NotifyItemModified(int index, string fieldPath = "")
        {
            if (index < 0 || index >= Count)
                return;
            
            NotifyItemsModified(index, index, fieldPath);
        }

        /// <summary>
        /// Informs observers that all items have been modified.
        /// </summary>
        public void NotifyItemsModified(string fieldPath = "")
        {
            if (Count <= 0)
                return;

            if (ItemsModified != null)
                ItemsModified(this, new DataItemsModifiedEventArgs(0, Count, fieldPath));
        }

        /// <summary>
        /// Informs observers that items have been modified.
        /// </summary>
        public void NotifyItemsModified(int startIndex, int endIndex, string fieldPath = "")
        {
            var args = new DataItemsModifiedEventArgs(startIndex, endIndex - startIndex + 1, fieldPath);

            for (var i = startIndex; i <= endIndex; i++)
            {
                ((ObservableItem) _list[i]).Modified(args);
            }

            if (ItemsModified != null)
                ItemsModified(this, args);
        }

        /// <summary>
        /// Returns list as read-only collection.
        /// </summary>
        public ObservableListWrapper<T> AsReadOnly() {
            return _readonlyList ?? (_readonlyList = new ObservableListWrapper<T>(this));
        }

        /// <summary>
        /// Performs a binary search on the sorted list using default comparer and returning a zero-based index of the item.
        /// </summary>
        public int BinarySearch(T item)
        {
            var searchItem = new ObservableItem(this, -1, item);
            return _list.BinarySearch(searchItem);
        }

        /// <summary>
        /// Performs a binary search on the sorted list using default comparer and returning a zero-based index of the item.
        /// </summary>
        public int BinarySearch(T item, IComparer<T> comparer)
        {
            var searchItem = new ObservableItem(this, -1, item);
            return _list.BinarySearch(searchItem, new ComparerConverter(comparer));
        }

        /// <summary>
        /// Performs a binary search on the sorted list using default comparer and returning a zero-based index of the item.
        /// </summary>
        public int BinarySearch(int index, int count, T item, IComparer<T> comparer) {
            var searchItem = new ObservableItem(this, -1, item);
            return _list.BinarySearch(index, count, searchItem, new ComparerConverter(comparer));
        }

        /// <summary>
        /// Removes all items from the list.
        /// </summary>
        public void Clear()
        {
            if (_list.Count == 0)
                return;

            var cleared = new List<IObservableItem>(_list.Count);
            foreach (var item in _list)
            {
                item.IsSelected = false;
                ((ObservableItem)item).Dispose();
                cleared.Add(item);
            }

            _list.Clear();
            _selectedItems.Clear();
            _selectedItem = null;

            if (ItemsRemoved != null)
            {
                ItemsRemoved(this,
                    new DataItemsRemovedEventArgs(DataItemsRemovedReason.Clear, 0, cleared.Count, cleared));
            }
        }

        /// <summary>
        /// Returns boolean indicating if list contains the item.
        /// </summary>
        public bool Contains(T item) {
            return item != null && _list.Exists(observable => Equals(observable.Value, item));
        }

        /// <summary>
        /// Returns boolean indicating if list contains the item.
        /// </summary>
        bool IObservableList.Contains(object item) {
            return Contains((T) item);
        }

        /// <summary>
        /// Converts the items in the list to another type and returns a new list.
        /// </summary>
        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            return _list.ConvertAll(input => converter((T)input.Value));
        }

        /// <summary>
        /// Copies the list to an array.
        /// </summary>
        public void CopyTo(T[] array)
        {
            CopyTo(array, 0);
        }

        /// <summary>
        /// Copies the list to an array.
        /// </summary>
        public void CopyTo(T[] array, int arrayIndex)
        {
            for (var i = 0; i < array.Length && i < _list.Count; i++)
            {
                array[i] = (T)_list[i].Value;
            }
        }

        /// <summary>
        /// Copies the list to an array.
        /// </summary>
        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            var j = 0;
            var i = index;
            for (; i < index + count; i++, j++)
            {
                array[arrayIndex + j] = (T) _list[i].Value;
            }
        }
        
        /// <summary>
        /// Returns boolean indicating if an item matching the predicate exists in the list.
        /// </summary>
        public bool Exists(Predicate<T> predicate)
        {
            return _list.Exists(item => predicate((T)item.Value));
        }

        /// <summary>
        /// Returns first item matching the predicate.
        /// </summary>
        public T Find(Predicate<T> predicate)
        {
            var result = _list.Find(item => predicate((T)item.Value));
            return result == null
                ? default(T)
                : (T)result.Value;
        }

        /// <summary>
        /// Returns all items that matches the predicate.
        /// </summary>
        public List<T> FindAll(Predicate<T> predicate)
        {
            var observables = _list.FindAll(item => predicate((T)item.Value));
            var result = new List<T>(observables.Count);
            foreach (var observ in observables)
            {
                result.Add((T) observ.Value);
            }
            return result;
        }

        /// <summary>
        /// Returns the index of the item matching the predicate.
        /// </summary>
        public int FindIndex(Predicate<T> predicate)
        {
            return _list.FindIndex(item => predicate((T)item.Value));
        }

        /// <summary>
        /// Returns the index of the item matching the predicate.
        /// </summary>
        public int FindIndex(int startIndex, Predicate<T> predicate)
        {
            return _list.FindIndex(startIndex, item => predicate((T)item.Value));
        }
        
        /// <summary>
        /// Returns the index of the item matching the predicate.
        /// </summary>
        public int FindIndex(int startIndex, int count, Predicate<T> predicate)
        {
            return _list.FindIndex(startIndex, count, item => predicate((T)item.Value));
        }

        /// <summary>
        /// Returns the last item matching the predicate.
        /// </summary>
        public T FindLast(Predicate<T> predicate) {
            var result = _list.FindLast(item => predicate((T)item.Value));
            return result == null
                ? default(T)
                : (T) result.Value;
        }

        /// <summary>
        /// Returns the index of the last item matching the predicate.
        /// </summary>
        public int FindLastIndex(Predicate<T> predicate)
        {
            return _list.FindLastIndex(item => predicate((T)item.Value));
        }

        /// <summary>
        /// Returns the index of the last item matching the predicate.
        /// </summary>
        public int FindLastIndex(int startIndex, Predicate<T> predicate)
        {
            return _list.FindLastIndex(startIndex, item => predicate((T)item.Value));
        }

        /// <summary>
        /// Returns the index of the last item matching the predicate.
        /// </summary>
        public int FindLastIndex(int startIndex, int count, Predicate<T> predicate)
        {
            return _list.FindLastIndex(startIndex, count, item => predicate((T) item.Value));
        }

        /// <summary>
        /// Performs an action on each item in the list.
        /// </summary>
        public void ForEach(Action<T> action)
        {
            _list.ForEach(item => action((T)item.Value));
        }

        /// <summary>
        /// Gets list enumerator.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return new EnumeratorConverter<T>(_list.GetEnumerator());
        }

        /// <summary>
        /// Gets list enumerator.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Creates a shallow copies of the specified range of items in the list.
        /// </summary>
        public List<T> GetRange(int index, int count)
        {
            var result = new List<T>(count);
            for (var i = index; i < _list.Count; i++)
            {
                result.Add((T)_list[i].Value);
            }
            return result;
        }

        /// <summary>
        /// Gets the index of the specified item.
        /// </summary>
        public int IndexOf(T item)
        {
            foreach (var observable in _list)
            {
                if (Equals(observable.Value, item))
                    return observable.Index;
            }
            return -1;
        }

        /// <summary>
        /// Gets the index of the specified item.
        /// </summary>
        public int IndexOf(T item, int startIndex)
        {
            for (var i = startIndex; i < _list.Count; i++)
            {
                if (Equals(_list[i].Value, item))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Gets the index of the specified item.
        /// </summary>
        public int IndexOf(T item, int startIndex, int count)
        {
            for (var i = startIndex; i < startIndex + count; i++)
            {
                if (Equals(_list[i].Value, item))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Inserts an item into the list at the specified index.
        /// </summary>
        public IObservableItem Insert(int index, T item)
        {
            var result = new ObservableItem(this, index, item);
            _list.Insert(index, result);

            // update index of items after inserted item
            for (var i = index + 1; i < _list.Count; i++)
            {
                ((ObservableItem)_list[i]).Index = i;
            }

            if (ItemsAdded != null)
                ItemsAdded(this, new DataItemsAddedEventArgs(DataItemAddReason.Insert, result));

            return result;
        }

        /// <summary>
        /// Inserts an item into the list at the specified index.
        /// </summary>
        void IList<T>.Insert(int index, T item)
        {
            Insert(index, item);
        }

        /// <summary>
        /// Inserts a range of items at the specified index.
        /// </summary>
        public IEnumerable<IObservableItem> InsertRange(int startIndex, IEnumerable<T> collection)
        {
            var itemCount = collection.Count();
            var endIndex = startIndex + (itemCount - 1);

            var result = new List<IObservableItem>(itemCount);
            var i = 0;
            foreach (var item in collection)
            {
                result.Add(new ObservableItem(this, startIndex + i, item));
                i++;
            }

            _list.InsertRange(startIndex, result);

            // update index of items after inserted items
            for (i = endIndex + 1; i < _list.Count; i++)
            {
                ((ObservableItem) _list[i]).Index = i;
            }

            if (ItemsAdded != null && result.Count > 0)
                ItemsAdded(this, new DataItemsAddedEventArgs(DataItemAddReason.Insert, startIndex, itemCount, result));

            return result;
        }

        /// <summary>
        /// Gets the last index of the specified item.
        /// </summary>
        public int LastIndexOf(T item)
        {
            return LastIndexOf(item, _list.Count - 1);
        }

        /// <summary>
        /// Gets the last index of the specified item.
        /// </summary>
        public int LastIndexOf(T item, int startIndex) {
            return LastIndexOf(item, startIndex, _list.Count - startIndex - 1);
        }

        /// <summary>
        /// Gets the last index of the specified item.
        /// </summary>
        public int LastIndexOf(T item, int startIndex, int count)
        {
            for (var i = startIndex; i >= startIndex - count; i--)
            {
                if (Equals(_list[i].Value, item))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Removes the first occurance of an item from the list.
        /// </summary>
        public bool Remove(object item)
        {
            return Remove((T)item);
        }

        /// <summary>
        /// Removes the first occurance of an item from the list.
        /// </summary>
        public bool Remove(T item)
        {
            var index = _list.IndexOf(new ObservableItem(this, -1, item));
            if (index < 0)
                return false;

            RemoveAt(index);
            return true;
        }
        
        /// <summary>
        /// Removes the items from the list.
        /// </summary>
        public void Remove(IEnumerable items)
        {
            var toRemove = new List<T>(items.OfType<T>());
            foreach (var item in toRemove)
            {
                Remove(item);
            }
        }

        /// <summary>
        /// Removes all items that matches the predicate.
        /// </summary>
        public int RemoveAll(Predicate<T> predicate)
        {
            var removedCount = 0;
            for (var i = _list.Count - 1; i >= 0; --i)
            {
                if (!predicate((T) _list[i].Value))
                    continue;

                RemoveAt(i);
                ++removedCount;
            }
            
            return removedCount;
        }

        /// <summary>
        /// Removes item at the specified index.
        /// </summary>
        public void RemoveAt(int index) {

            var old = _list[index];
            old.IsSelected = false;
            ((ObservableItem)old).Dispose();

            _list.RemoveAt(index);

            for (var i = index; i < _list.Count; i++)
            {
                ((ObservableItem) _list[i]).Index = i;
            }

            if (ItemsRemoved != null)
                ItemsRemoved(this, new DataItemsRemovedEventArgs(DataItemsRemovedReason.Remove, old));
        }

        /// <summary>
        /// Removes a range of items from the list.
        /// </summary>
        public void RemoveRange(int startIndex, int count)
        {
            var oldRange = _list.GetRange(startIndex, count);
            foreach (var old in oldRange)
            {
                old.IsSelected = false;
                ((ObservableItem)old).Dispose();
            }

            _list.RemoveRange(startIndex, count);

            for (var i = startIndex; i < _list.Count; i++)
            {
                ((ObservableItem) _list[i]).Index = i;
            }

            if (ItemsRemoved != null && oldRange.Count > 0)
            {
                ItemsRemoved(this, new DataItemsRemovedEventArgs(
                    DataItemsRemovedReason.Remove, oldRange[0].Index, oldRange.Count, oldRange));
            }
        }

        /// <summary>
        /// Reverses the order of the list.
        /// </summary>
        public void Reverse()
        {
            _list.Reverse();

            for (var i = 0; i < _list.Count; i++)
            {
                ((ObservableItem) _list[i]).Index = i;
            }

            if (ItemsMoved != null)
                ItemsMoved(this, new DataItemsMovedEventArgs(DataItemsMoveReason.Reverse, 0, Count));
        }

        /// <summary>
        /// Reverses the order of the items in the specified range.
        /// </summary>
        public void Reverse(int startIndex, int count)
        {
            _list.Reverse(startIndex, count);

            for (var i = startIndex; i < startIndex + count; i++)
            {
                ((ObservableItem) _list[i]).Index = i;
            }

            if (ItemsMoved != null)
                ItemsMoved(this, new DataItemsMovedEventArgs(DataItemsMoveReason.Reverse, startIndex, count));
        }

        /// <summary>
        /// Sorts the list using the default comparer.
        /// </summary>
        public void Sort()
        {
            _list.Sort();

            for (var i = 0; i < _list.Count; i++)
            {
                ((ObservableItem) _list[i]).Index = i;
            }

            if (ItemsMoved != null)
                ItemsMoved(this, new DataItemsMovedEventArgs(DataItemsMoveReason.Sort, 0, Count));
        }

        /// <summary>
        /// Sorts the list.
        /// </summary>
        public void Sort(Comparison<T> comparison)
        {
            _list.Sort((x, y) => comparison((T) x.Value, (T) y.Value));

            for (var i = 0; i < _list.Count; i++)
            {
                ((ObservableItem) _list[i]).Index = i;
            }

            if (ItemsMoved != null)
                ItemsMoved(this, new DataItemsMovedEventArgs(DataItemsMoveReason.Sort, 0, Count));
        }

        /// <summary>
        /// Sorts the list.
        /// </summary>
        public void Sort(IComparer<T> comparer)
        {
            _list.Sort(new ComparerConverter(comparer));

            for (var i = 0; i < _list.Count; i++)
            {
                ((ObservableItem) _list[i]).Index = i;
            }

            if (ItemsMoved != null)
                ItemsMoved(this, new DataItemsMovedEventArgs(DataItemsMoveReason.Sort, 0, Count));
        }

        /// <summary>
        /// Sorts the list.
        /// </summary>
        public void Sort(int startIndex, int count, IComparer<T> comparer)
        {
            _list.Sort(startIndex, count, new ComparerConverter(comparer));

            for (var i = startIndex; i < startIndex + count; i++)
            {
                ((ObservableItem) _list[i]).Index = i;
            }

            if (ItemsMoved != null)
                ItemsMoved(this, new DataItemsMovedEventArgs(DataItemsMoveReason.Sort, startIndex, count));
        }

        /// <summary>
        /// Copies the list to an array.
        /// </summary>
        public T[] ToArray() {
            var result = new T[_list.Count];
            for (var i = 0; i < _list.Count; i++)
            {
                result[i] = (T) _list[i].Value;
            }
            return result;
        }

        /// <summary>
        /// Sets capacity to the number of items in the list.
        /// </summary>
        public void TrimExcess()
        {
            _list.TrimExcess();
        }

        /// <summary>
        /// Returns boolean indicating if all items matches the predicate.
        /// </summary>
        public bool TrueForAll(Predicate<T> predicate)
        {
            return _list.TrueForAll(item => predicate((T)item.Value));
        }

        /// <summary>
        /// Gets index of an item.
        /// </summary>
        public int GetIndex(object item)
        {
            return item != null ? IndexOf((T)item) : -1;
        }

        /// <summary>
        /// Get observable container for specified item. Returns null if the item is not
        /// in the list.
        /// </summary>
        public IObservableItem GetObservable(object item)
        {
            foreach (var observable in _list)
            {
                if (Equals(observable.Value, item))
                    return observable;
            }
            return null;
        }

        /// <summary>
        /// Scrolls to item.
        /// </summary>
        public void ScrollTo(T item)
        {
            ScrollTo(item, ElementAlignment.Center, new ElementMargin());
        }

        /// <summary>
        /// Scrolls to item.
        /// </summary>
        public void ScrollTo(T item, ElementAlignment alignment, ElementMargin offset)
        {
            var index = IndexOf(item);
            if (index >= 0)
            {
                ScrollTo(index, alignment, offset);
            }
        }

        /// <summary>
        /// Scrolls to item.
        /// </summary>
        public void ScrollTo(int index)
        {
            ScrollTo(index, ElementAlignment.Center, new ElementMargin());
        }

        /// <summary>
        /// Scrolls to item.
        /// </summary>
        public void ScrollTo(int index, ElementAlignment alignment, ElementMargin offset)
        {
            var args = new DataScrollToEventArgs(index, alignment, offset);
            if (ScrolledTo != null)
                ScrolledTo(this, args);

            LastScroll = args;
        }

        private void CallSelectChangedEvent(IObservableItem item, DataItemSelectChangedEventArgs args)
        {
            if (ItemSelectChanged != null)
                ItemSelectChanged(item, args);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the capacity of the list.
        /// </summary>
        public int Capacity
        {
            get
            {
                return _list.Capacity;
            }
            set
            {
                _list.Capacity = value;
            }
        }

        /// <summary>
        /// Gets the number of items in the list.
        /// </summary>
        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        /// <summary>
        /// Determine if collection is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Returns list as observable items read-only collection.
        /// </summary>
        public IList<IObservableItem> Observables
        {
            get { return _list.AsReadOnly(); }
        }

        public IList<object> Values
        {
            get { return _values ?? (_values =new ListConverter<object>(_list)); }
        }

        /// <summary>
        /// Gets item at index.
        /// </summary>
        object IObservableList.this[int index]
        {
            get { return this[index]; }
        }

        /// <summary>
        /// Gets or sets the item at the specified index.
        /// </summary>
        public T this[int index]
        {
            get { return (T)_list[index].Value; }
            set
            {
                var old = _list[index];
                if (ReferenceEquals(old.Value, value))
                    return;

                old.IsSelected = false;
                ((ObservableItem)old).Dispose();

                _list[index] = new ObservableItem(this, index, value);

                if (ItemsRemoved != null)
                    ItemsRemoved(this, new DataItemsRemovedEventArgs(DataItemsRemovedReason.Replace, old));

                if (ItemsAdded != null)
                    ItemsAdded(this, new DataItemsAddedEventArgs(DataItemAddReason.Replace, _list[index]));
            }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public T SelectedItem
        {
            get { return _selectedItem != null ? (T)_selectedItem.Value : default(T); }
            set
            {
                if (_selectedItem != null && ReferenceEquals(_selectedItem.Value, value))
                    return;

                if (value == null)
                {
                    SelectedIndex = -1;
                    return;
                }

                var selectedItem = GetObservable(value);
                SelectedIndex = selectedItem != null ? selectedItem.Index : -1;
            }
        }

        /// <summary>
        /// Gets or sets the selected observable item.
        /// </summary>
        public IObservableItem SelectedObservable
        {
            get { return _selectedItem; }
            set
            {
                if (ReferenceEquals(_selectedItem, value))
                    return;

                if (value == null)
                {
                    SelectedIndex = -1;
                    return;
                }

                SelectedIndex = value.Index;
            }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        object IObservableList.SelectedItem
        {
            get { return SelectedItem; }
            set { SelectedItem = value == null ? default(T) : (T)value; }
        }

        /// <summary>
        /// Gets or sets the selected index.
        /// </summary>
        public int SelectedIndex
        {
            get { return _selectedItem != null ? _selectedItem.Index : -1; }
            set
            {
                if (_selectedItem != null && _selectedItem.Index == value)
                    return;

                _selectedItem = value >= 0 ? _list[value] : null;

                var prev = new List<IObservableItem>(_selectedItems);
                _selectedItems.Clear();

                if (_selectedItem != null)
                {
                    _selectedItems.Add(_selectedItem);
                    _selectedItem.ForceSelected(true);
                }

                foreach (var item in prev)
                {
                    if (item.Index != value)
                        item.IsSelected = false;
                }
            }
        }

        /// <summary>
        /// Get all selected items.
        /// </summary>
        public ICollection<T> SelectedItems
        {
            get { return _tSelectedItems
                         ?? (_tSelectedItems = new ListConverter<T>(_selectedItems)); }
        }

        /// <summary>
        /// Get all selected items.
        /// </summary>
        ICollection<object> IObservableList.SelectedItems
        {
            get { return _objSelectedItems
                         ?? (_objSelectedItems = new ListConverter<object>(_selectedItems)); }
        }

        /// <summary>
        /// Returns selected observable items list as read-only collection.
        /// </summary>
        public ICollection<IObservableItem> SelectedObservables
        {
            get { return _selectedItems.AsReadOnly(); }
        }

        public DataScrollToEventArgs LastScroll
        {
            get; private set;
        }

        #endregion


        #region Private Classes

        private class ObservableItem : IObservableItem, IDisposable
        {
            public event EventHandler<DataItemSelectChangedEventArgs> ItemSelectChanged;
            public event EventHandler<DataItemIndexChangedEventArgs> ItemIndexChanged;
            public event EventHandler<DataItemsModifiedEventArgs> ItemModified;

            private readonly ObservableList<T> _model;
            private bool _isSelected;
            private int _index;
            private readonly T _value;
            private bool _isDisposed;

            public ObservableItem(ObservableList<T> list, int index, T value) {
                _model = list;
                _index = index;
                _value = value;
            }

            public override int GetHashCode() {
                return _value == null ? 0 : _value.GetHashCode();
            }

            public override bool Equals(object obj) {
                var other = obj as IObservableItem;
                return Equals(_value, other != null ? other.Value : obj);
            }

            public IObservableList DataModel
            {
                get { return _model; }
            }

            public int Index
            {
                get { return _index; }
                set
                {
                    if (_index == value || _isDisposed)
                        return;

                    var old = _index;
                    _index = value;

                    if (ItemIndexChanged != null)
                        ItemIndexChanged(this, new DataItemIndexChangedEventArgs(old, _index));
                }
            }

            public object Value
            {
                get { return _value; }
            }

            public bool IsSelected
            {
                get { return _isSelected; }
                set
                {
                    if (_isSelected == value)
                        return;

                    ForceSelected(value);
                }
            }

            public void ForceSelected(bool isSelected) {

                if (_isDisposed)
                    return;

                _isSelected = isSelected;

                if (_isSelected)
                {
                    _model._selectedItem = this;

                    if (!_model._selectedItems.Contains(this))
                        _model._selectedItems.Add(this);
                }
                else
                {
                    if (Equals(this, _model._selectedItem))
                        _model._selectedItem = _model._selectedItems.LastOrDefault();

                    _model._selectedItems.Remove(this);
                }

                var eventArgs = new DataItemSelectChangedEventArgs(Index, isSelected);

                if (ItemSelectChanged != null)
                    ItemSelectChanged(this, eventArgs);

                _model.CallSelectChangedEvent(this, eventArgs);
            }

            public void NotifyModified() {
                _model.NotifyItemModified(Index);
            }

            public void ScrollTo()
            {
                ScrollTo(ElementAlignment.Center, new ElementMargin());
            }

            public void ScrollTo(ElementAlignment alignment, ElementMargin offset) {
                _model.ScrollTo(Index, alignment, offset);
            }

            public void Dispose() {
                _isDisposed = true;
            }

            public void Modified(DataItemsModifiedEventArgs args) {
                if (ItemModified != null)
                    ItemModified(this, args);
            }
        }


        /// <summary>
        /// Convert T comparer to ObservableListItem comparer
        /// </summary>
        private class ComparerConverter : IComparer<IObservableItem>
        {
            private readonly IComparer<T> _comparer;

            public ComparerConverter(IComparer<T> comparer) {
                _comparer = comparer;
            }

            public int Compare(IObservableItem x, IObservableItem y) {
                return _comparer.Compare((T)x.Value, (T)y.Value);
            }
        }

        /// <summary>
        /// Convert IObservableListItem comparer to T comparer
        /// </summary>
        private class EnumeratorConverter<T1> : IEnumerator<T1>
        {
            private readonly IEnumerator<IObservableItem> _enumerator;

            public EnumeratorConverter(IEnumerator<IObservableItem> enumerator) {
                _enumerator = enumerator;
            }

            public bool MoveNext() {
                return _enumerator.MoveNext();
            }

            public void Reset() {
                _enumerator.Reset();
            }

            public T1 Current
            {
                get
                {
                    var current = _enumerator.Current;
                    return current == null
                        ? default(T1)
                        : (T1)current.Value;
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public void Dispose() {
                _enumerator.Dispose();
            }
        }

        private class ListConverter<T1> : IList<T1>
        {
            private readonly IList<IObservableItem> _items;

            public ListConverter(IList<IObservableItem> items)
            {
                _items = items;
            }

            IEnumerator<T1> IEnumerable<T1>.GetEnumerator()
            {
                return new EnumeratorConverter<T1>(_items.GetEnumerator());
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new EnumeratorConverter<T>(_items.GetEnumerator());
            }

            public void Add(T1 item)
            {
                throw new NotImplementedException();
            }

            void ICollection<T1>.Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(T1 item)
            {
                object obj = item;
                var searchTerm = new ObservableItem(null, -1, (T)obj);
                return _items.Contains(searchTerm);
            }

            public void CopyTo(T1[] array, int arrayIndex)
            {
                for (var i = 0; i < array.Length && i < _items.Count; i++)
                {
                    array[i] = (T1)_items[i].Value;
                }
            }

            public bool Remove(T1 item)
            {
                throw new NotImplementedException();
            }

            int ICollection<T1>.Count
            {
                get { return _items.Count; }
            }

            bool ICollection<T1>.IsReadOnly
            {
                get { return true; }
            }

            public int IndexOf(T1 item) {
                object obj = item;
                return _items.IndexOf(new ObservableItem(null, -1, (T)obj));
            }

            public void Insert(int index, T1 item) {
                throw new NotImplementedException();
            }

            public void RemoveAt(int index) {
                throw new NotImplementedException();
            }

            public T1 this[int index]
            {
                get { return (T1)_items[index].Value; }
                set { throw new NotImplementedException(); }
            }
        }

        #endregion
    }
}
