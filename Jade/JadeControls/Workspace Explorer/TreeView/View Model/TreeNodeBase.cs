using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;

namespace JadeControls.Workspace.ViewModel
{
    public abstract class TreeNodeBase : NotifyPropertyChanged
    {
        #region Data

        private bool _selected;
        private bool _expanded;
        private TreeNodeBase _parent;
        private string _displayName;
        /// <summary>
        /// Child Tree nodes
        /// </summary>
        private ObservableCollection<TreeNodeBase> _children;

        #endregion

        #region Constructor

        protected TreeNodeBase(string displayName, TreeNodeBase parent)
        {
            _displayName = displayName;
            _selected = false;
            _expanded = false;
            _parent = parent;
            _children = new ObservableCollection<TreeNodeBase>();
        }

        #endregion

        #region Public Properties

        public TreeNodeBase Parent { get { return _parent; } }

        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    OnPropertyChanged("Selected");
                }
            }
        }

        public bool Expanded
        {
            get { return _expanded; }
            set 
            {
                if (_expanded != value)
                {
                    if (value == false && _parent == null)
                    {
                        //can't colapse the root item
                        return;
                    }

                    _expanded = value;
                    this.OnPropertyChanged("Expanded");

                    // Expand all the way up to the root.
                    if (_expanded && _parent != null)
                        _parent.Expanded = true;
                }
            }
        }

        public ObservableCollection<TreeNodeBase> Children 
        { 
            get { return _children; }
            protected set { _children = value; }
        }

        public string DisplayName
        {
            get { return _displayName; }
        }
        
        #endregion

        #region Public Methods

        protected virtual bool RemoveChildData(TreeNodeBase child) 
        {
            return true;
        }

        public bool RemoveChild(TreeNodeBase child)
        {
            if (Children.Contains(child) && RemoveChildData(child))
            {
                Children.Remove(child);
                OnPropertyChanged("Children");
                return true;
            }
            return false;
        }

        public bool ContainsChild(string displayName)
        {
            TreeNodeBase child = Children.Where(a => a.DisplayName == displayName).FirstOrDefault();
            return child != null;
        }

        public TreeNodeBase FindSelected()
        {
            if (Selected)
                return this;

            foreach (TreeNodeBase child in Children)
            {
                TreeNodeBase sel = child.FindSelected();
                if (sel != null)
                    return sel;
            }
            return null;
        }

        #endregion
    }
}
