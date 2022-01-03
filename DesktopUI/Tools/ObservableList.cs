using System.Collections.Generic;
using System.Collections.Specialized;

namespace DesktopUI.Tools
{
    public class ObservableList<T> : List<T>, INotifyCollectionChanged
    {
        public ObservableList()
        {
        }

        public ObservableList(IEnumerable<T> collection) : base(collection)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public ObservableList(int capacity) : base(capacity)
        {
        }

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        new public void Add(T item)
        {
            base.Add(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void AddRange(IEnumerable<T> items)
        {
            foreach(var item in items)
            {
                base.Add(item);
            }
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
