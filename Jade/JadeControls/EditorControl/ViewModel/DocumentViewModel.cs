using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Threading;
using System.Windows.Threading;
using JadeCore;
using JadeUtils.IO;

namespace JadeControls.EditorControl.ViewModel
{
    
    //wrapps an IEditorDoc and associated CodeEditor view 
    public class DocumentViewModel : ViewModelBase
    {
        #region Data

        private IEditorDoc _document;
        private bool _selected;

        //The view's view of the content
        private ICSharpCode.AvalonEdit.Document.TextDocument _avDoc;
        private CodeEditor _view;
        private JadeCore.Editor.CodeLocation _caretLocation;

        //private DocViewModelCommandAdaptor _commands;

        #endregion

        #region Constructor

        protected DocumentViewModel(IEditorDoc doc, CodeEditor view)
        {
            _document = doc;
            _document.OnSaved += delegate { OnPropertyChanged("Modified"); };
            _view = view;
            _selected = false;
            _caretLocation = new JadeCore.Editor.CodeLocation(0, 0, 0);
            _view.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            _avDoc = new ICSharpCode.AvalonEdit.Document.TextDocument(_document.Content);
            _avDoc.TextChanged += _avDoc_TextChanged;
            
        }

        #endregion

        #region Public Properties

    //    public DocViewModelCommandAdaptor Commands { get { return _commands; } }

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
                if (value != _selected)
                {
                    _selected = value;
                    OnPropertyChanged("Selected");
                }
            } 
        }

        public JadeCore.Editor.CodeLocation CaretLocation
        {
            get { return _caretLocation;}
            set
            {
                if (_view.CaretOffset != value.Offset)
                {
                    _view.CaretOffset = value.Offset;
                    OnPropertyChanged("CaretLocation");
                }
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

        #region Public Methods

        /// <summary>
        /// Display the specified location
        /// </summary>
        /// <param name="loc"></param>
        public void DisplayLocation(JadeCore.Editor.CodeLocation loc)
        {
            CaretLocation = loc;
            _view.TextArea.Focus();           
        }

        public void HighlightRange(int startOffset, int endOffset)
        {
            _view.Select(startOffset, endOffset - startOffset);
        }

        #endregion

        #region Document Event Handlers

        private void _avDoc_TextChanged(object sender, EventArgs e)
        {
            bool m = Modified;
            _document.Content = _avDoc.Text;
            if (!m)
                OnPropertyChanged("Modified");
        }

        #endregion

        #region View Event Handlers

        void Caret_PositionChanged(object sender, EventArgs e)
        {
            if (_caretLocation.Offset != _view.TextArea.Caret.Offset)
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
        }

        #endregion
    }
}
