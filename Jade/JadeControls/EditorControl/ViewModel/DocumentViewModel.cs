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
        private CodeEditor _view;
        private JadeCore.Editor.CodeLocation _caretLocation;

        #endregion

        #region Constructor

        public DocumentViewModel(IEditorDoc doc, CodeEditor codeEditor)
        {
            _document = doc;
            _document.OnSaved += delegate { OnPropertyChanged("Modified"); };
            _view = codeEditor;
            _selected = false;
            _caretLocation = new JadeCore.Editor.CodeLocation(0, 0, 0);
            _view.TextArea.Caret.PositionChanged += Caret_PositionChanged;
        }

        void Caret_PositionChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("Line {0} Col {1} Offset{2}", 
                                            _view.TextArea.Caret.Line,
                                            _view.TextArea.Caret.Column,
                                            _view.TextArea.Caret.Offset));
            _caretLocation.Line = _view.TextArea.Caret.Line;
            _caretLocation.Column = _view.TextArea.Caret.Column;
            _caretLocation.Offset = _view.TextArea.Caret.Offset;
            OnPropertyChanged("CaretLocation");
        }

        #endregion

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
                if (_selected && _avDoc == null)
                {
                    _avDoc = new ICSharpCode.AvalonEdit.Document.TextDocument(_document.Content);
                    _avDoc.TextChanged += _avDoc_TextChanged;
                    
                }
                OnPropertyChanged("Selected");
            } 
        }

        public JadeCore.Editor.CodeLocation CaretLocation
        {
            get { return _caretLocation;}
            set
            {
                _view.CaretOffset = value.Offset;
            }
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

        private void _avDoc_TextChanged(object sender, EventArgs e)
        {
            bool m = Modified;
            _document.Content = _avDoc.Text;
            if (!m)
                OnPropertyChanged("Modified");
        }

    }
}
