using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.IO;
using JadeUtils.IO;

namespace JadeControls.EditorControl.ViewModel
{
    public class DocumentViewModel : ViewModelBase
    {
        #region Data

        private string _name;
        private FilePath _path;
        private bool _selected;
        private ICSharpCode.AvalonEdit.Document.TextDocument _avDoc;

        #endregion

        public DocumentViewModel(string name, FilePath path)
        {
            _name = name;
            _path = path;
            _selected = false;
          //  _avDoc = new ICSharpCode.AvalonEdit.Document.TextDocument();
        }

        public override string ToString()
        {
            return DisplayName;
        }

        #region Public Properties

        public JadeUtils.IO.FilePath Path { get { return _path; } }

        public override string DisplayName { get { return _name; } }

        public bool Selected 
        { 
            get 
            { 
                return _selected; 
            } 
            set 
            { 
                _selected = value; 
                OnPropertyChanged("Selected");
                if (_selected && _avDoc == null)
                {
                    using (FileStream fs = new FileStream(_path.Str, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (StreamReader reader = ICSharpCode.AvalonEdit.Utils.FileReader.OpenStream(fs, System.Text.Encoding.UTF8))
                        {
                            _avDoc = new ICSharpCode.AvalonEdit.Document.TextDocument(reader.ReadToEnd());
                        }
                    }
                }
            } 
        }

        public ICSharpCode.AvalonEdit.Document.TextDocument Document
        {
            get { return _avDoc; }
        }

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
