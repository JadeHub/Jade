using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.ComponentModel;
using System.Text;

namespace JadeCore
{    
    public class RecentFileList
    {
        private int _maxItems;
        private ObservableCollection<string> _files;
        private ReadOnlyObservableCollection<string> _readOnlyFiles;

        public RecentFileList(int maxItems = 5)
        {
            _maxItems = maxItems;
            _files = new ObservableCollection<string>();
            _readOnlyFiles = new ReadOnlyObservableCollection<string>(_files);
        }

        public ReadOnlyObservableCollection<string> Files //{ get { return _readOnlyFiles; } }
        {
            get 
            {
                return _readOnlyFiles;
            }
        }

        public void Load(System.Collections.Specialized.StringCollection values)
        {
            _files.Clear();
            foreach (string path in values)
            {
                Add(path);
            }
        }

        public void Save(System.Collections.Specialized.StringCollection values)
        {
            values.Clear();
            values.AddRange(_files.ToArray());
        }

        public void Add(string path)
        {
            _files.Remove(path);
            if (_files.Count == _maxItems)
            {
                _files.RemoveAt(_maxItems - 1);
            }
            _files.Insert(0, path);
        }
    }
}
