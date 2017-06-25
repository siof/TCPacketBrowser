using siof.Common.Extensions;
using siof.Common.Locks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Windows.Threading;

namespace siof.Common.Wpf
{
    public class DispatchedObservableCollection<T> : IList<T>, IList, INotifyCollectionChanged, INotifyPropertyChanged
    {
        protected IList<T> _collection;
        private Dispatcher _dispatcher;

        public Dispatcher CurrentDispatcher
        {
            get { return _dispatcher; }
            set { _dispatcher = value; }
        }

        private DispatcherPriority _dispatcherPriority = DispatcherPriority.Background;

        public DispatcherPriority DispatcherPriority
        {
            get { return _dispatcherPriority; }
            set { _dispatcherPriority = value; }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        [NonSerialized]
        protected ReaderWriterLockSlim _collectionLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public DispatchedObservableCollection(IEnumerable<T> col, Dispatcher dis = null)
        {
            _dispatcher = dis ?? Dispatcher.CurrentDispatcher;
            if (col != null)
                _collection = new List<T>(col);
            else
                _collection = new List<T>();
        }

        public DispatchedObservableCollection(Dispatcher dis = null)
        {
            _dispatcher = dis ?? Dispatcher.CurrentDispatcher;
            _collection = new List<T>();
        }

        protected object DispatcherInvoke(Delegate method, params object[] args)
        {
            try
            {
                return _dispatcher.Invoke(method, DispatcherPriority, args);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public int BinarySearch(T value)
        {
            using (new ReadLock(_collectionLock))
            {
                return ((List<T>)_collection).BinarySearch(value);
            }
        }

        public void Sort(bool disableNotify = false)
        {
            using (new WriteLock(_collectionLock))
            {
                ((List<T>)_collection).Sort();

                if (!disableNotify)
                    foreach (var item in _collection)
                    {
                        InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
                    }
            }
        }

        public void NotifyReset()
        {
            if (CollectionChanged == null)
                return;

            if (Thread.CurrentThread == _dispatcher.Thread)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
            else
            {
                DispatcherInvoke((Action)(() => { NotifyReset(); }));
            }
        }

        private char _toStringSeparator = ';';

        public char ToStringSeparator
        {
            get { return _toStringSeparator; }
            set { _toStringSeparator = value; }
        }

        public void Add(T item)
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                DoAdd(item);
            else
                DispatcherInvoke((Action)(() => { DoAdd(item); }));
        }

        private void DoAdd(T item)
        {
            using (new WriteLock(_collectionLock))
            {
                _collection.Add(item);
            }
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            OnPropertyChanged(() => Count);
            OnPropertyChanged(() => HasItems);
        }

        public void AddRange(IEnumerable items)
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                DoAddRange(items);
            else
                DispatcherInvoke(new Action<IEnumerable<T>, bool>(DoAddRange), items, false);
        }

        public void ReplaceAll(IEnumerable<T> newItems)
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                DoReplaceAll(newItems, false);
            else
                DispatcherInvoke(new Action<IEnumerable<T>, bool>(DoReplaceAll), newItems, false);
        }

        private void DoReplaceAll(IEnumerable<T> newItems, bool disableNotify)
        {
            using (new WriteLock(_collectionLock))
            {
                _collection.Clear();
                _collection = new List<T>(newItems);
            }

            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(() => Count);
            OnPropertyChanged(() => HasItems);
        }

        public void AddRange(IEnumerable items, bool disableNotify)
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                DoAddRange(items, disableNotify);
            else
                DispatcherInvoke(new Action<IEnumerable<T>, bool>(DoAddRange), items, disableNotify);
        }

        public DispatchedObservableCollection<T> AddRangeAndReturn(IEnumerable<T> items)
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                DoAddRange(items);
            else
                DispatcherInvoke(new Action<IEnumerable<T>, bool>(DoAddRange), items);

            return this;
        }

        private void DoAddRange(IEnumerable items, bool disableNotify = false)
        {
            List<T> itemsT;
            using (new WriteLock(_collectionLock))
            {
                itemsT = items.OfType<T>().ToList();
                foreach (var item in itemsT)
                {
                    _collection.Add(item);
                }
            }

            if (!disableNotify)
                foreach (var item in itemsT)
                {
                    InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, itemsT));
                }

            OnPropertyChanged(() => Count);
            OnPropertyChanged(() => HasItems);
        }

        public void Clear()
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                DoClear();
            else
                DispatcherInvoke((Action)(() => { DoClear(); }));
        }

        private void DoClear()
        {
            using (new WriteLock(_collectionLock))
            {
                _collection.Clear();
            }

            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            OnPropertyChanged(() => Count);
            OnPropertyChanged(() => HasItems);
        }

        public void ClearAndAdd(T item)
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                DoClearAndAdd(item);
            else
                DispatcherInvoke(new Action<T>(DoClearAndAdd), item);
        }

        public void DoClearAndAdd(T item)
        {
            using (new WriteLock(_collectionLock))
            {
                _collection.Clear();
                _collection.Add(item);
            }

            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(() => Count);
            OnPropertyChanged(() => HasItems);
        }

        public void ClearAndAddRange(IEnumerable items)
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                DoClearAndAddRange(items);
            else
                DispatcherInvoke(new Action<IEnumerable<T>>(DoClearAndAddRange), items);
        }

        private void DoClearAndAddRange(IEnumerable items)
        {
            using (new WriteLock(_collectionLock))
            {
                _collection.Clear();
                if (items != null)
                {
                    var itemsT = items.OfType<T>().ToList();
                    _collection.AddRange(itemsT);
                }
            }
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            OnPropertyChanged(() => Count);
            OnPropertyChanged(() => HasItems);
        }

        public bool Contains(T item)
        {
            using (new ReadLock(_collectionLock))
            {
                var result = _collection.Contains(item);
                return result;
            }
        }

        public T Find(Predicate<T> pred)
        {
            using (new ReadLock(_collectionLock))
            {
                var result = ((List<T>)_collection).Find(pred);
                return result;
            }
        }

        /// <summary>
        /// Pobiera listę elementów i tworzy ich kopię referencji
        /// </summary>
        /// <returns></returns>
        public T FirstOrDefault(Func<T, bool> filter)
        {
            T ret = default(T);
            using (new ReadLock(_collectionLock))
            {
                ret = this._collection.FirstOrDefault(filter);
            }
            return (ret);
        }

        public void ForEachSync(Action<T> action)
        {
            using (new ReadLock(_collectionLock))
            {
                _collection.ForEach(action);
            }
        }

        public IList<T> WhereSync(Func<T, bool> filter)
        {
            using (new ReadLock(_collectionLock))
            {
                return _collection.Where(filter).ToList();
            }
        }

        public IList<TResult> SelectSync<TResult>(Func<T, TResult> selector)
        {
            using (new ReadLock(_collectionLock))
            {
                return _collection.Select(selector).ToList();
            }
        }

        public IList<TResult> WhereSelectSync<TResult>(Func<T, bool> filter, Func<T, TResult> selector)
        {
            using (new ReadLock(_collectionLock))
            {
                return _collection.Where(filter).Select(selector).ToList();
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            using (new WriteLock(_collectionLock))
            {
                _collection.CopyTo(array, arrayIndex);
            }
        }

        public void Update(ICollection<T> updateCollection, Func<T, int> keyExpression)
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                DoUpdate(updateCollection, keyExpression);
            else
                DispatcherInvoke(new Action<ICollection<T>, Func<T, int>>(DoUpdate), updateCollection, keyExpression);
        }

        private void DoUpdate(ICollection<T> updateCollection, Func<T, int> keyExpression)
        {
            List<NotifyCollectionChangedEventArgs> events = new List<NotifyCollectionChangedEventArgs>(updateCollection.Count);
            using (new WriteLock(_collectionLock))
            {
                foreach (var item in updateCollection)
                {
                    int key = keyExpression(item);
                    T oldItem = _collection.FirstOrDefault(en => keyExpression(en) == key);
                    if (oldItem == null)
                    {
                        _collection.Add(item);
                        events.Add(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
                    }
                    else
                    {
                        if (oldItem.Equals(item) == false)
                        {
                            int oldItemIndex = _collection.IndexOf(oldItem);
                            if (oldItemIndex >= 0)
                            {
                                _collection[oldItemIndex] = item;
                                events.Add(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, oldItem, oldItemIndex));
                            }
                        }
                    }
                }
            }
            if (CollectionChanged != null)
                events.ForEach(e => InvokeCollectionChanged(e));
            OnPropertyChanged(() => Count);
            OnPropertyChanged(() => HasItems);
        }

        public int Count
        {
            get
            {
                using (new ReadOnlyLock(_collectionLock))
                {
                    var result = _collection.Count;
                    return result;
                }
            }
        }

        public bool HasItems
        {
            get
            {
                using (new ReadOnlyLock(_collectionLock))
                {
                    var result = _collection.Any();
                    return result;
                }
            }
        }

        public bool IsReadOnly
        {
            get { return _collection.IsReadOnly; }
        }

        public bool Remove(T item)
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                return DoRemove(item);
            else
            {
                var op = DispatcherInvoke(new Func<T, bool>(DoRemove), item);
                if (op != null)
                {
                    return (bool)op;
                }
            }
            return false;
        }

        private bool DoRemove(T item)
        {
            bool result = false;
            int index = -1;
            using (new WriteLock(_collectionLock))
            {
                index = _collection.IndexOf(item);
                if (index == -1)
                    return false;
                result = _collection.Remove(item);
            }

            try
            {
                if (result && CollectionChanged != null)
                    InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            }
            catch
            { }

            OnPropertyChanged(() => Count);
            OnPropertyChanged(() => HasItems);
            return result;
        }

        public void RemoveRange(IEnumerable removeCollection, bool disableNotify = false)
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                DoRemoveRange(removeCollection, disableNotify);
            else
                DispatcherInvoke(new Action<IEnumerable<T>, bool>(DoRemoveRange), removeCollection, disableNotify);
        }

        private void DoRemoveRange(IEnumerable removeCollection, bool disableNotify = false)
        {
            List<T> itemsT = null;
            using (new WriteLock(_collectionLock))
            {
                itemsT = removeCollection.OfType<T>().Where(i => _collection.Contains(i)).ToList();
                _collection.RemoveRange(itemsT);
            }

            try
            {
                if (CollectionChanged != null && !disableNotify)
                    itemsT.ForEach(item => InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item)));
            }
            catch
            { }

            OnPropertyChanged(() => Count);
            OnPropertyChanged(() => HasItems);
        }

        public IEnumerator<T> GetEnumerator()
        {
            using (new ReadOnlyLock(_collectionLock))
            {
                IList<T> itms = _collection.ToList();
                return itms.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            using (new ReadOnlyLock(_collectionLock))
            {
                IList<T> itms = _collection.ToList();
                return itms.GetEnumerator();
            }
        }

        public int IndexOf(T item)
        {
            using (new ReadOnlyLock(_collectionLock))
            {
                var result = _collection.IndexOf(item);
                return result;
            }
        }

        public void Insert(int index, T item)
        {
            if (index >= 0)
            {
                if (Thread.CurrentThread == _dispatcher.Thread)
                    DoInsert(index, item);
                else
                    DispatcherInvoke((Action)(() => { DoInsert(index, item); }));
            }
            else
            {
                Insert(0, item);
            }
        }

        private void DoInsert(int index, T item)
        {
            using (new WriteLock(_collectionLock))
            {
                _collection.Insert(index, item);
            }
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));

            OnPropertyChanged(() => Count);
            OnPropertyChanged(() => HasItems);
        }

        public T GetOrInsert(Func<T, bool> func, int index, T item)
        {
            if (index >= 0)
            {
                if (Thread.CurrentThread == _dispatcher.Thread)
                    return DoGetOrInsert(func, index, item);
                else
                    return (T)DispatcherInvoke((Func<T>)(() => DoGetOrInsert(func, index, item)));
            }
            else
            {
                return GetOrInsert(func, 0, item);
            }
        }

        private T DoGetOrInsert(Func<T, bool> func, int index, T item)
        {
            using (new WriteLock(_collectionLock))
            {
                T el = _collection.FirstOrDefault(func);
                if (el != null)
                    return el;
                _collection.Insert(index, item);
            }
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));

            OnPropertyChanged(() => Count);
            OnPropertyChanged(() => HasItems);

            return item;
        }

        public void Move(T item, int newIndex)
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                DoMove(item, newIndex);
            else
                DispatcherInvoke((Action)(() => { DoMove(item, newIndex); }));
        }

        private void DoMove(T item, int newIndex)
        {
            int oldIndex = -1;
            using (new WriteLock(_collectionLock))
            {
                oldIndex = _collection.IndexOf(item);
                _collection.RemoveAt(oldIndex);
                _collection.Insert(newIndex, item);
            }
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex));
        }

        public void Move(int oldIndex, int newIndex)
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                DoMove(oldIndex, newIndex);
            else
                DispatcherInvoke((Action)(() => { DoMove(oldIndex, newIndex); }));
        }

        private void DoMove(int oldIndex, int newIndex)
        {
            T item;
            using (new WriteLock(_collectionLock))
            {
                item = _collection[oldIndex];
                _collection.RemoveAt(oldIndex);
                _collection.Insert(newIndex, item);
            }
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex));
        }

        public void RemoveAt(int index)
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                DoRemoveAt(index);
            else
                DispatcherInvoke((Action)(() => { DoRemoveAt(index); }));
        }

        private void DoRemoveAt(int index)
        {
            T item;
            using (new WriteLock(_collectionLock))
            {
                if (_collection.Count == 0 || _collection.Count <= index)
                    return;
                item = _collection[index];
                _collection.RemoveAt(index);
            }

            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));

            OnPropertyChanged(() => Count);
            OnPropertyChanged(() => HasItems);
        }

        public void Replace(T oldItem, T newItem)
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                DoReplace(oldItem, newItem);
            else
                DispatcherInvoke((Action)(() => { DoReplace(oldItem, newItem); }));
        }

        private void DoReplace(T oldItem, T newItem)
        {
            int oldItemIndex = -1;
            using (new WriteLock(_collectionLock))
            {
                oldItemIndex = _collection.IndexOf(oldItem);
                if (oldItemIndex >= 0)
                    _collection[oldItemIndex] = newItem;
                else
                    _collection.Add(newItem);
            }
            if (oldItemIndex >= 0)
            {
                InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, oldItemIndex));
            }
            else
            {
                InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem));
            }
            OnPropertyChanged(() => Count);
            OnPropertyChanged(() => HasItems);
        }

        public T this[int index]
        {
            get
            {
                using (new ReadOnlyLock(_collectionLock))
                {
                    T result = default(T);
                    if (_collection.Count > index)
                        result = _collection[index];
                    return result;
                }
            }

            set
            {
                using (new WriteLock(_collectionLock))
                {
                    if (_collection.Count == 0 || _collection.Count <= index)
                    {
                        return;
                    }
                    _collection[index] = value;
                }
            }
        }

        public int Add(object value)
        {
            if (value is T)
            {
                Add((T)value);
                return Count - 1;
            }
#if DEBUG
            throw new Exception(string.Format("Invalid item type!!!! Collection element type: {0}. Add element type {1}", typeof(T).Name, value != null ? value.GetType().Name : "NULL"));
#endif
            return -1;
        }

        public bool Contains(object value)
        {
            if (value is T)
            {
                return Contains((T)value);
            }
            return false;
        }

        public int IndexOf(object value)
        {
            if (value is T)
            {
                return IndexOf((T)value);
            }
            return -1;
        }

        public void Insert(int index, object value)
        {
            if (value is T)
            {
                Insert(index, (T)value);
            }
            else
            {
#if DEBUG
                throw new Exception(string.Format("Invalid item type!!!! Collection element type: {0}. Insert element type {1}", typeof(T).Name, value != null ? value.GetType().Name : "NULL"));
#endif
            }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            if (value is T)
            {
                Remove((T)value);
            }
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                if (value is T)
                {
                    this[index] = (T)value;
                }
                else
                {
#if DEBUG
                    throw new Exception(string.Format("Invalid item type!!!! Collection element type: {0}. Add element type {1}", typeof(T).Name, value != null ? value.GetType().Name : "NULL"));
#endif
                }
            }
        }

        public void CopyTo(Array array, int index)
        {
            if (array is T[])
            {
                CopyTo((T[])array, index);
            }
        }

        public bool IsSynchronized
        {
            get { return true; }
        }

        public object SyncRoot
        {
            get { return _collectionLock; }
        }

        protected virtual void OnPropertyChanged(Expression<Func<object>> property)
        {
            PropertyChangedEventHandler handler = System.Threading.Interlocked.CompareExchange(ref PropertyChanged, null, null);
            if (handler != null)
            {
                string propName = property.GetPropertyName();
                handler(this, new PropertyChangedEventArgs(propName));
            }
        }

        protected void InvokeCollectionChanged(NotifyCollectionChangedEventArgs ev)
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                DoCollectionChanged(ev);
            else
                DispatcherInvoke(new Action<NotifyCollectionChangedEventArgs>(DoCollectionChanged), ev);
        }

        private void DoCollectionChanged(NotifyCollectionChangedEventArgs ev)
        {
            try
            {
                if (CollectionChanged != null)
                    CollectionChanged(this, ev);
            }
            catch (Exception ex)
            {
            }
        }

        public override string ToString()
        {
            string result = null;
            if (Thread.CurrentThread == _dispatcher.Thread)
                result = _collection.ToString(ToStringSeparator);
            else
            {
                using (new ReadOnlyLock(_collectionLock))
                {
                    result = _collection.ToString(ToStringSeparator);
                }
            }
            return result;
        }
    }
}
