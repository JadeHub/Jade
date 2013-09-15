using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace JadeControls.EditorControl.ViewModel
{
    public class DocumentViewModel : ViewModelBase, JadeCore.ViewModels.IEditorDocument
    {
        #region Data

        private string _name;
        private string _text;
        private bool _selected;

        #endregion

        public DocumentViewModel(string name, string text)
        {
            _name = name;
            _text = text;
            _selected = false;
        }

        public override string ToString()
        {
            return DisplayName;
        }

        #region Public Properties

        public override string DisplayName { get { return _name; } }
        public string Text { get { return _text; } set { _text = value; OnPropertyChanged("Text"); } }
        public bool Selected { get { return _selected; } set { _selected = value; OnPropertyChanged("Selected"); } }

        #endregion

        #region Close Command

        private RelayCommand _closeCommand;

        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand(param => this.OnCloseCommand(), param => this.CanDoCloseCommand);
                }
                return _closeCommand;
            }
        }

        public void OnCloseCommand()
        {
            EventHandler h = RequestClose;
            if (h != null)
                h(this, EventArgs.Empty);
        }

        private bool CanDoCloseCommand
        {
            get { return true; }
        }

        #endregion

        #region Events

        public event EventHandler RequestClose;

        #endregion
    }
}
