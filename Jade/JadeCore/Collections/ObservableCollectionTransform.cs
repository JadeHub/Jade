using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;

namespace JadeCore.Collections
{
    public class ObservableCollectionTransform<FromT, ToT> : ObservableCollection<ToT> where FromT : class
    {
        private INotifyCollectionChanged _Source;
        private Func<FromT, ToT> _Transformer;

        public ObservableCollectionTransform(INotifyCollectionChanged source, Func<FromT, ToT> transformer)
        {
            _Source = source;
            _Transformer = transformer;
            source.CollectionChanged += OnSourceCollectionChanged;
        }

        void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {            
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        Debug.Assert(e.NewItems[i] is FromT);
                        this.InsertItem(e.NewStartingIndex + i, _Transformer(e.NewItems[i] as FromT));
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    // Remove the item from our collection
                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        this.RemoveAt(e.OldStartingIndex + i);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.Clear();
                    break;
                default:
                    throw new NotSupportedException(e.Action.ToString());
            }
        }
    }
}
