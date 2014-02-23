using System;
using System.Collections.ObjectModel;
using System.Text;

namespace JadeControls.OutputControl.ViewModel
{
    public class OutputItemViewModel
    {
        private JadeCore.Output.IItem _Item;

        public OutputItemViewModel(JadeCore.Output.IItem item)
        {
            _Item = item;
        }

        private string GetLevelText(JadeCore.Output.Level level)
        {
            switch (level)
            {
                case(JadeCore.Output.Level.Crit):
                    return "Critical:    ";
                case (JadeCore.Output.Level.Err):
                    return "Error:       ";
                case (JadeCore.Output.Level.Info):
                    return "Information: ";
                case (JadeCore.Output.Level.Warn):
                    return "Warning:     ";

            }
            throw new InvalidOperationException("Bad Level");
        }

        public string DisplayText
        {
            get
            {
                return _Item.Message;
            }
        }
    }

    public class OutputViewModel : JadeControls.Docking.ToolPaneViewModel
    {
        private JadeCore.Output.IOutputController _Controller;
        private JadeCore.Collections.ObservableCollectionTransform<JadeCore.Output.IItem, OutputItemViewModel> _Items;
        private StringBuilder _sb;

        public OutputViewModel(JadeCore.Output.IOutputController controller)
        {
            Title = "Output";
            ContentId = "OutputToolPane";
            _Controller = controller;
            _Items = new JadeCore.Collections.ObservableCollectionTransform<JadeCore.Output.IItem, OutputItemViewModel>(_Controller.Items, 
                delegate (JadeCore.Output.IItem i){ return new OutputItemViewModel(i); });
            _Items.CollectionChanged += ItemsCollectionChanged;
            _sb = new StringBuilder();
        }

        void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                if (Text.Length > 0)
                    _sb.Append(Environment.NewLine);
                _sb.Append(((OutputItemViewModel)e.NewItems[0]).DisplayText);
                
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                _sb = new StringBuilder();
            }
            else 
            {
                RebuildString();
            }
            OnPropertyChanged("Text");
        }

        private void RebuildString()
        {
            _sb = new StringBuilder();
            bool first = true;
            foreach (OutputItemViewModel i in _Items)
            {
                if (!first)
                {
                    _sb.Append(Environment.NewLine);

                }
                else
                {
                    first = false;
                }
                _sb.Append(i.DisplayText);

            }
        }

        public ObservableCollection<OutputItemViewModel> Items
        {
            get { return _Items; }
        }

        public string Text
        {
            get
            {
                return _sb.ToString();
            }            
        }
    }
}
