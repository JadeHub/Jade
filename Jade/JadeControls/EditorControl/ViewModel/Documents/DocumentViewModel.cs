using JadeCore;
using System;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;

namespace JadeControls.EditorControl.ViewModel
{
    //wrapps an IEditorDoc and associated CodeEditor view 
    /// <summary>
    /// Handles Caret location & focus
    /// </summary>
    public abstract class DocumentViewModel : Docking.PaneViewModel
    {
        #region CaretLocation

        private class CaretLocation
        {
            private readonly ITextDocument _doc;
            private int _offset;
            private int _line;
            private int _column;

            public CaretLocation(ITextDocument doc)
            {
                _doc = doc;
                _offset = _line = _column = -1;
            }

            public int Offset
            {
                get
                {
                    return _offset;
                }

                set
                {
                    if (_offset != value)
                    {
                        _offset = value;
                        _column = -1;
                        _line = -1;
                    }
                }
            }

            public int Column
            {
                get
                {
                    if (_column == -1)
                        Calculate();
                    return _column;
                }

            }

            public int Line
            {
                get
                {
                    if (_line == -1)
                        Calculate();
                    return _line;
                }

            }

            private void Calculate()
            {
                if (_offset == -1)
                    return;
                _line = _doc.GetLineNumForOffset(_offset);
                ISegment lineSeg = _doc.GetLineForOffset(_offset);
                _column = _offset - lineSeg.Offset + 1;
            }
        }

        #endregion

        #region Data

        static ImageSourceConverter ISC = new ImageSourceConverter();
               
        /// <summary>
        /// The doc from the EditorController
        /// </summary>
        private IEditorDoc _model;
        private CodeEditor _view;

        /// <summary>
        /// Current location of cursor
        /// </summary>
        private CaretLocation _caretLocation;

        private bool _wantInitialFocus;
                
        #endregion

        #region Constructor

        protected DocumentViewModel(IEditorDoc doc)
        {
            Title = doc.Name;
            ContentId = doc.File.ToString();
            IconSource = ISC.ConvertFromInvariantString("pack://application:,,,/Images/File.png") as ImageSource;
            _model = doc;
            _model.TextDocument.ModifiedChanged += OnTextDocumentModifiedChanged;
            _caretLocation = new CaretLocation(_model.TextDocument);
            CaretOffset = 0;
            _wantInitialFocus = true;
        }

        #endregion

        private void OnTextDocumentModifiedChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("Modified");
        }

        public virtual void SetView(CodeEditor view)
        {
            _view = view;
            _view.Document = _model.TextDocument.AvDoc;

            //BindCommands(_view.CommandBindings);

            //Initialise the view's caret location in case CaretOffset has already been set.
            _view.CaretOffset = _caretLocation.Offset;
            //Track changes in the caret location
            _view.TextArea.Caret.PositionChanged += Caret_PositionChanged;

            _view.Loaded += OnViewLoaded;

            //Let any derived class initialise
            OnSetView(_view);
        }

        private void OnViewLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_wantInitialFocus)
                Keyboard.Focus(_view.TextArea);
        }
        
        protected virtual void OnSetView(CodeEditor view)
        {
        }

        private void OnSearchCommand()
        {
            SearchCurrentFile.SearchCurrentFileDialog dlg = new SearchCurrentFile.SearchCurrentFileDialog();

            dlg.Show();
        }

        #region Public Properties

        public bool Modified
        {
            get { return _model.Modified; }           
        }

        public event EventHandler CaretOffsetChanged;

        private void RaiseCaretOffsetChangedEvent()
        {
            EventHandler handler = CaretOffsetChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public int CaretOffset
        {
            get { return _caretLocation.Offset; }
            set
            {
                if(_caretLocation.Offset != value)
                {
                    _caretLocation.Offset = value;
                    if (_view != null)
                        _view.CaretOffset = _caretLocation.Offset;
                    OnPropertyChanged("CaretOffset");
                    OnPropertyChanged("CaretLine");
                    OnPropertyChanged("CaretColumn");

                    RaiseCaretOffsetChangedEvent();
                }
            }
        }

        public int CaretLine
        {
            get { return _caretLocation.Line; }
        }

        public int CaretColumn
        {
            get { return _caretLocation.Column; }
        }
       
        public IEditorDoc Document
        {
            get { return _model; }
        }

        public ITextDocument TextDocument
        {
            get { return _model.TextDocument; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Display the specified location
        /// </summary>
        /// <param name="loc"></param>
        public void DisplayLocation(int offset, bool setFocus, bool scroll)
        {
            CaretOffset = offset;
            if (_view != null)
            {
                if (setFocus)
                    _view.TextArea.Focus();
                if(scroll)
                {
                    int line = TextDocument.GetLineNumForOffset(offset);
                    double visualTop = _view.TextArea.TextView.GetVisualTopByDocumentLine(line);
                    _view.ScrollToVerticalOffset(visualTop);
                }
            }
            else
            {
                _wantInitialFocus = setFocus;
            }
        }

        public void HighlightRange(int startOffset, int endOffset)
        {
            _view.Select(startOffset, endOffset - startOffset);
        }

        #endregion

        #region Document Event Handlers

        #endregion

        #region View Event Handlers

        void Caret_PositionChanged(object sender, EventArgs e)
        {
            if (_caretLocation.Offset != _view.TextArea.Caret.Offset)
            {
                /*System.Diagnostics.Debug.WriteLine(string.Format("Line {0} Col {1} Offset{2}",
                                                _view.TextArea.Caret.Line,
                                                _view.TextArea.Caret.Column,
                                                _view.TextArea.Caret.Offset));*/
                CaretOffset = _view.TextArea.Caret.Offset;
            }
        }

        #endregion

        //public abstract void BindCommands(CommandBindingCollection commandBindings);
    }
}
