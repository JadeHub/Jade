using JadeCore;
using System;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace JadeControls.EditorControl.ViewModel
{    
    //wrapps an IEditorDoc and associated CodeEditor view 
    public abstract class DocumentViewModel : Docking.PaneViewModel
    {
        #region Data

        static ImageSourceConverter ISC = new ImageSourceConverter();

        private IEditorDoc _document;
        private bool _selected;

        //The view's view of the content
        private ICSharpCode.AvalonEdit.Document.TextDocument _avDoc;
        private CodeEditor _view;
        private JadeCore.Editor.CodeLocation _caretLocation;
                
        #endregion

        #region Constructor

        protected DocumentViewModel(IEditorDoc doc)
        {
            Title = doc.Name;
            ContentId = doc.Path.Str;
            IconSource = ISC.ConvertFromInvariantString("pack://application:,,,/Images/File.png") as ImageSource;
            _document = doc;
            _document.OnSaved += delegate { OnPropertyChanged("Modified"); };
            _selected = false;
            _caretLocation = new JadeCore.Editor.CodeLocation(0, 0, 0);
        }

        public void SetView(CodeEditor view)
        {
            _view = view;
            _avDoc = _view.Document;

            //Load the document
            _avDoc.Text = _document.Content;
            _avDoc.TextChanged += _avDoc_TextChanged;
            
            _view.CaretOffset = _caretLocation.Offset;
            _view.TextArea.Caret.PositionChanged += Caret_PositionChanged;
        }

        #endregion

        #region Public Properties

        public string Path { get { return _document.Path.Str; } }

        public bool Modified
        {
            get { return _document.Modified; }
            set
            {
                if (_document.Modified != value)
                {
                    _document.Modified = value;
                    OnPropertyChanged("Modified");
                    OnPropertyChanged("DisplayName");
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
                if (_caretLocation.Offset != value.Offset)
                {
                    _caretLocation = value;
                    if(_view != null)
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
           // _view.TextArea.Focus();           
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

        public abstract void RegisterCommands(CommandBindingCollection commandBindings);
    }
}
