using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeControls.ContextTool
{
    public interface ITreeItem
    {
        string Name { get; }
        string TypeChar { get; }
        ObservableCollection<ITreeItem> Children { get; }
        
        ITreeItem Parent { get; }
        ITreeItem FindSelected();
        ITreeItem FindChild(string name);

        bool Selected { get; set; }
        bool Expanded { get; set; }
    }

    public abstract class TreeItemBase : NotifyPropertyChanged, ITreeItem
    {
        private ObservableCollection<ITreeItem> _children;
        private string _name;
        private ITreeItem _parent;

        protected TreeItemBase(ITreeItem parent, string name)
        {
            _children = new ObservableCollection<ITreeItem>();
            _name = name;
            _parent = parent;
        }

        public string Name { get { return _name; } }

        public ObservableCollection<ITreeItem> Children { get { return _children; } }

        public bool Selected { get; set ; }
        public bool Expanded { get; set; }

        public virtual string TypeChar { get { return ""; } }

        public ITreeItem Parent { get { return _parent; } }

        public ITreeItem FindSelected()
        {
            if (Selected) return this;

            foreach(ITreeItem child in Children)
            {
                ITreeItem result = child.FindSelected();
                if (result != null) return result;
            }
            return null;
        }

        public ITreeItem FindChild(string name)
        {
            foreach(ITreeItem item in Children)
            {
                if (name == item.Name)
                    return item;
            }
            return null;
        }
    }
}
