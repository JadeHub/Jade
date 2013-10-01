using System;
using System.Collections.Generic;
using System.Windows.Input;
using JadeCore;
using JadeUtils.IO;

namespace JadeControls.EditorControl.ViewModel
{
    public class DocumentViewModel : ViewModelBase
    {
        #region Data

        private IEditorDoc _document;
        private bool _selected;
        private ICSharpCode.AvalonEdit.Document.TextDocument _avDoc;

        #endregion

      //  public event EventHandler OnDocumentClosing;

        #region Constructor

        public DocumentViewModel(IEditorDoc doc)
        {
            _document = doc;
            //_document.OnClosing += delegate(object sender, EventArgs args) { OnDocumentClosing(sender, args); };
            _selected = false;
        }

        #endregion

        public override string ToString()
        {
            return DisplayName;
        }

        #region Public Properties

        public string Path { get { return _document.Path.Str; } }
        
        public override string DisplayName { get { return _document.Name; } }

        public bool Modified
        {
            get { return _document.Modified; }

            set
            {
                if (_document.Modified != value)
                {
                    _document.Modified = value;
                    OnPropertyChanged("Modified");
                }
            }
        }

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
                    _avDoc = new ICSharpCode.AvalonEdit.Document.TextDocument(_document.Content);
                    _avDoc.TextChanged += _avDoc_TextChanged;
                }
            } 
        }

        void _avDoc_TextChanged(object sender, EventArgs e)
        {
            _document.Content = _avDoc.Text;
        }

        public ICSharpCode.AvalonEdit.Document.TextDocument TextDocument
        {
            get { return _avDoc; }
        }

        public IEditorDoc Document
        {
            get { return _document; }
        }

        #endregion        
        
    }
}
