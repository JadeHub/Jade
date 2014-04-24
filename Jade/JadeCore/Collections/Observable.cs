using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore.Collections.Observable
{
    public delegate void ListItemAddedEventHandler<ItemT>(ItemT item);
    public delegate void ListItemRemovedEventHandler<ItemT>(ItemT item);

    public class List<ItemT> : IEnumerable<ItemT>
    {
        #region Data

        private IList<ItemT> _list;

        #endregion

        #region Events

        public ListItemAddedEventHandler<ItemT> ItemAdded;
        public ListItemRemovedEventHandler<ItemT> ItemRemoved;

        public void RaiseItemAdded(ItemT item)
        {
            ListItemAddedEventHandler<ItemT> h = ItemAdded;
            if (h != null)
                h(item);
        }

        public void RaiseItemRemoved(ItemT item)
        {
            ListItemRemovedEventHandler<ItemT> h = ItemRemoved;
            if (h != null)
                h(item);
        }

        #endregion

        public List()
        {
            _list = new System.Collections.Generic.List<ItemT>();
        }

        public void Add(ItemT item)
        {
            _list.Add(item);
            RaiseItemAdded(item);
        }

        public bool Contains(ItemT item)
        {
            return _list.Contains(item);
        }

        public bool Remove(ItemT item)
        {
            return _list.Remove(item);
        }

        public void Observe(ListItemAddedEventHandler<ItemT> onAdded, ListItemRemovedEventHandler<ItemT> onRemoved)
        {
            foreach(ItemT item in _list)
            {
                onAdded(item);
            }
            ItemAdded += onAdded;
            ItemRemoved += onRemoved;
        }
                
        #region IEnumerable

        public IEnumerator<ItemT> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion
    }

    public delegate void MapItemAddedEventHandler<ValueT>(ValueT v);
    public delegate void MapItemRemovedEventHandler<KeyT>(KeyT k);
    
    public class Map<KeyT, ValueT>
    {
        private Dictionary<KeyT, ValueT> _map;

        public MapItemAddedEventHandler<ValueT> ItemAdded;
        public MapItemRemovedEventHandler<KeyT> ItemRemoved;

        public void RaiseItemAdded(ValueT v)
        {
            MapItemAddedEventHandler<ValueT> h = ItemAdded;
            if (h != null)
                h(v);
        }

        public void RaiseItemRemoved(KeyT k)
        {
            MapItemRemovedEventHandler<KeyT> h = ItemRemoved;
            if (h != null)
                h(k);
        }

        public Map()
        {
            _map = new Dictionary<KeyT, ValueT>();
        }

        public bool AddItem(KeyT key, ValueT val)
        {
            if (_map.ContainsKey(key))
            {
                _map[key] = val;
                return false;
            }
            _map.Add(key, val);
            RaiseItemAdded(val);
            return true;
        }

        public bool RemoveItem(KeyT k)
        {
            bool result = _map.Remove(k);
            if (result)
                RaiseItemRemoved(k);
            return result;
        }

        public ValueT Get(KeyT k)
        {
            ValueT val;
            _map.TryGetValue(k, out val);
            return val;
        }

        public IEnumerable<KeyT> Keys { get { return _map.Keys; } }
        public IEnumerable<ValueT> Values { get { return _map.Values; } }
    }
}
