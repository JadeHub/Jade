using System;
using System.Diagnostics;
using System.Collections.Generic;
using LibClang;

namespace CppCodeBrowser.Symbols
{
    public interface ISymbolSet<T> : IEnumerable<T> where T : ISymbol
    {
        void Update(T item);
        T Find(string usr);
    }

    public class SymbolSet<T> : ISymbolSet<T> where T : ISymbol
    {
        private IDictionary<string, T> _symbols;
        private Func<Cursor, T> _creator;

        public SymbolSet(Func<Cursor, T> creator)
        {
            _creator = creator;
            _symbols = new Dictionary<string, T>();
        }

        #region IEnumerable<T>
        /// <summary>
        /// Gets an enumerator for this list.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return _symbols.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _symbols.Values.GetEnumerator();
        }
        #endregion


        public void Update(T item)
        {
            Cursor c = item.Cursor;
            Debug.Assert(c.Usr.Length > 0);
            _symbols.Add(c.Usr, item);
        }

        public T Find(string usr)
        {
            T result = default(T);
            _symbols.TryGetValue(usr, out result);
            return result;
        }

        public Tuple<bool, T> FindOrAdd(Cursor c)
        {
            T result = Find(c.Usr);
            if (result != null) return new Tuple<bool, T>(false, result);
            result = _creator(c);
            Update(result);
            return new Tuple<bool, T>(true, result);
        }
    }
}
